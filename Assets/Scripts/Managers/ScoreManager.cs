using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Managers {
	public class ScoreManager : ManagerBase<ScoreManager> {

		private const int SCORE_MAX = 999999999;
		public int score { get; private set; } = 0;
		private ScriptManager _scriptManager = null;

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
		public override void OnDestroyManager() {}
		public override void OnArchiveLoaded() {}
		public override void OnArchiveSaveBegin() {}
	}
}