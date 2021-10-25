using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using GCL.Serialization;
using UnityEngine;

namespace Game.Managers {
	public class ScoreManager : ManagerBase<ScoreManager> {

		private const int SCORE_MAX = 999999999;
		public int score { get; private set; } = 0;
		private ScriptManager _scriptManager = null;
		private Dictionary<int, int> _classicHighestScoreDict = new Dictionary<int, int>();
		private int _infiniteHighestScore = 0;

		public void InitScore() {
			score = 0;
		}

		public void AddScore(int brickID, DamageResult result, int hitCount) {
			var ret = _scriptManager.ExecuteWithCache("score.peak", "calc_score",
				new ScriptValue(score),
				new ScriptValue(brickID),
				new ScriptValue(result.damageValue),
				new ScriptValue(result.bDie),
				new ScriptValue(hitCount));
			_ChangeScore((int) ret.GetNumber(score));
		}

		public int GetHighestScore(MapMode mode, int mapID) {
			if (mode == MapMode.Classic) {
				if (_classicHighestScoreDict.TryGetValue(mapID, out var ret)) {
					return ret;
				}
			} else if (mode == MapMode.Infinite) {
				return _infiniteHighestScore;
			} else if (mode == MapMode.Challenge) {
				if (_classicHighestScoreDict.TryGetValue(mapID, out var ret)) {
					return ret;
				}
			}
			return 0;
		}

		public bool OverScore(MapMode mode, int mapID) {
			var highest = GetHighestScore(mode, mapID);
			if (score > highest) {
				SetHighestScore(mode, mapID, score);
				return true;
			}
			InitScore();
			return false;
		}

		public void SetHighestScore(MapMode mode, int mapID, int score) {
			if (mode == MapMode.Classic) {
				_classicHighestScoreDict[mapID] = score;
			} else if (mode == MapMode.Infinite) {
				_infiniteHighestScore = score;
			} else if (mode == MapMode.Challenge) {
				_classicHighestScoreDict[mapID] = score;
			}
		}

		private void _ChangeScore(int value) {
			var pre = score;
			score = Mathf.Clamp(value, 0, SCORE_MAX);
			if (pre != score) {
				MessageCenter.Send(new MessageScoreChange(score, pre, score - pre));
			}
		}

		public override void OnInitManager() {
			_scriptManager = ManagerCenter.GetManager<ScriptManager>();
		}
		public override void OnDestroyManager() {
		}
		public override void OnArchiveLoaded(ArchiveSystem.Archive archive) {
			var src = archive.GetString("ScoreManager", "HighestScore", "");
			if (!string.IsNullOrEmpty(src)) {
				var ret = JSONTool.ParseToCustomKV<int, int>(src);
				if (ret != null) {
					_classicHighestScoreDict = ret;
					foreach (var item in _classicHighestScoreDict) {
						Debug.Log(item.Key + "," + item.Value);
					}
				}
			}
		}
		public override void OnArchiveSaveBegin(ArchiveSystem.Archive archive) {
			var src = JSONTool.ToString(_classicHighestScoreDict);
			archive.SetString("ScoreManager", "HighestScore", src);
		}
	}
}