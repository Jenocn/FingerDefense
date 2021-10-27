using System.Collections.Generic;
using Game.Tables;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public class GameStateChallenge : GameStateBase {
		private List<int> _normalIDs = new List<int>();
		private int _index = 0;

		public override void OnMessageBrickHit(MessageBrickHit msg) {
			scoreManager.AddScore(mapManager.mapMode, msg.uniqueID, msg.damageResult, hitCount);
		}
		public override void OnMessageRacketHit(MessageRacketHit msg) {}
		public override void OnMessageFallIntoTrap(MessageFallIntoTrap msg) {}

		public override void OnGameControllerEvent(GameController.Event e) {
			if (e == GameController.Event.BrickZero) {
				controller.Stop();
				if (_index >= _normalIDs.Count) {
					bool bHighest = scoreManager.OverScore(mapManager.mapMode, mapManager.currentID);
					MessageCenter.Send(new MessageGameOver(
						mapManager.mapMode, mapManager.currentID,
						scoreManager.score, highHitCount, true, bHighest));
				} else {
					controller.CountDown(3, () => {
						_Generate();
						controller.Run();
					});
				}
			} else if (e == GameController.Event.BallZero) {
				controller.Stop();

				bool bHighest = scoreManager.OverScore(mapManager.mapMode, mapManager.currentID);
				MessageCenter.Send(new MessageGameOver(
					mapManager.mapMode, mapManager.currentID,
					scoreManager.score, highHitCount, false, bHighest));
			}
		}
		public override void OnGamePause(bool bPause) {}

		private bool _Generate() {
			for (var i = _index; i < _normalIDs.Count; ++i) {
				var element = _GetElement(i);
				if (element) {
					foreach (var item in element.items) {
						controller.CreateBrick(item.id, item.hpMax, item.position);
					}
					_index = i + 1;
					return true;
				}
			}
			return false;
		}

		private TableMapdatElement _GetElement(int index) {
			if (index >= 0 && index < _normalIDs.Count) {
				return TableMapdat.instance.GetElement(_normalIDs[index]);
			}
			return null;
		}

		public override void OnStateCreate() {
			base.OnStateCreate();
		}
		public override void OnStateStart() {
			base.OnStateStart();
			_normalIDs = TableMapdat.instance.GetNormalIDs();
			_index = 0;
			_Generate();
		}
		public override void OnStateDestroy() {
			base.OnStateDestroy();
		}
		public override void OnStateUpdate() {
			base.OnStateUpdate();
		}
	}
}