using System.Collections.Generic;
using Game.Managers;
using Game.Tables;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public class GameStateInfinite : GameStateBase {

		private TableMapdatElement _templateElement = null;
		private HashSet<int> _invalidSet = new HashSet<int>();
		private float _generateStartPercent = 0.2f;
		private float _generateInterval = 0;
		private int _generateCount = 1;
		private float _duration = 0;

		public override void OnMessageBrickHit(MessageBrickHit msg) {
			if (msg.damageResult.bDie) {}

			scoreManager.AddScore(mapManager.mapMode, msg.uniqueID, msg.damageResult, hitCount);
		}
		public override void OnMessageRacketHit(MessageRacketHit msg) {}
		public override void OnMessageFallIntoTrap(MessageFallIntoTrap msg) {}
		public override void OnGameControllerEvent(GameController.Event e) {
			if (e == GameController.Event.BallZero) {
				controller.Stop();
				bool bHighest = scoreManager.OverScore(mapManager.mapMode, mapManager.currentID);
				MessageCenter.Send(new MessageGameOver(
					mapManager.mapMode, mapManager.currentID,
					scoreManager.score, highHitCount, false, bHighest));
			}
		}
		public override void OnGamePause(bool bPause) {

		}

		public override void OnStateCreate() {
			base.OnStateCreate();
			_generateStartPercent = TableConfig.GetFloat("infinite_generate_start_percent");
			_generateInterval = TableConfig.GetFloat("infinite_generate_interval");
			_generateCount = TableConfig.GetInt("infinite_generate_count");

			scriptManager.message.AddListener<PeakMessage_CreateInfiniteBrick>(_invalidSet, (PeakMessage_CreateInfiniteBrick msg) => {
				_ScriptGenerateBrick(msg.index, msg.brickID, msg.hpMax, msg.position);
			});
		}
		public override void OnStateStart() {
			base.OnStateStart();

			_templateElement = TableMapdat.instance.GetInfiniteElement(1);

			_Generate((int) (_templateElement.items.Count * _generateStartPercent));
		}
		public override void OnStateDestroy() {
			base.OnStateDestroy();

			scriptManager.message.RemoveListener<PeakMessage_CreateInfiniteBrick>(_invalidSet);
		}

		public override void OnStateUpdate() {
			base.OnStateUpdate();

			if (!controller.isStop) {
				_duration += Time.deltaTime;

				if (_duration >= _generateInterval) {
					_Generate(_generateCount);
					_duration = 0;
				}
			}
		}

		private void _ScriptGenerateBrick(int index, int id, int hpMax, Vector2 position) {
			if (controller.CreateBrick(id, hpMax, position, () => {
					_invalidSet.Remove(index);
				})) {
				_invalidSet.Add(index);
			}
		}

		private void _Generate(int count) {
			var randomList = new List<int>();
			for (var i = 0; i < _templateElement.items.Count; ++i) {
				if (!_invalidSet.Contains(i)) {
					randomList.Add(i);
				}
			}

			for (int i = 0; i < count; ++i) {
				if (randomList.Count == 0) {
					return;
				}

				var num = Random.Range(0, randomList.Count);
				var index = randomList[num];
				var item = _templateElement.items[index];
				scriptTrigger.ExecuteFunction("infinite_generate",
					new peak.interpreter.ValueNumber(index),
					new peak.interpreter.ValueNumber(item.position.x),
					new peak.interpreter.ValueNumber(item.position.y));
				randomList.Remove(index);
			}
		}
	}
}