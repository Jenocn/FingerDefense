using System.Collections.Generic;
using Game.Managers;
using Game.Tables;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public class GameStateInfinite : GameStateBase {

		private TableMapdatElement _templateElement = null;
		private HashSet<int> _invalidSet = new HashSet<int>();

		public override void OnMessageBrickHit(MessageBrickHit msg) {
			if (msg.damageResult.bDie) {}

			scoreManager.AddScore(mapManager.mapMode, msg.uniqueID, msg.damageResult, hitCount);
		}
		public override void OnMessageRacketHit(MessageRacketHit msg) {}
		public override void OnMessageFallIntoTrap(MessageFallIntoTrap msg) {}
		public override void OnGameControllerEvent(GameController.Event e) {
			if (e == GameController.Event.BallZero) {
				controller.Stop();
				MessageCenter.Send(new MessageGameOver(
					mapManager.mapMode, mapManager.currentID,
					scoreManager.score, highHitCount, false, false));
			}
		}

		public override void OnStateCreate() {
			base.OnStateCreate();
		}
		public override void OnStateStart() {
			base.OnStateStart();

			_templateElement = TableMapdat.instance.GetInfiniteElement(1);

			_Generate(_templateElement.items.Count / 4);
		}
		public override void OnStateDestroy() {
			base.OnStateDestroy();
		}

		private float _duration = 0;
		public override void OnStateUpdate() {
			base.OnStateUpdate();

			_duration += Time.deltaTime;
			if (_duration >= 0.25f) {
				_Generate(1);
				_duration = 0;
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
				controller.CreateBrick(Random.Range(1, 6), Random.Range(1, 4), item.position, () => {
					_invalidSet.Remove(index);
				});
				randomList.Remove(index);
				_invalidSet.Add(index);
			}
		}
	}
}