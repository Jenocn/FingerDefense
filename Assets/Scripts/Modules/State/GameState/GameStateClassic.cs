using Game.Tables;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public class GameStateClassic : GameStateBase {
		private int _brickAmount = 0;
		private bool _bOver = false;

		public override void OnMessageBrickHit(MessageBrickHit msg) {
			if (msg.damageResult.bDie) {
				--_brickAmount;

				if (_brickAmount == 0) {
					if (!_bOver) {
						_bOver = true;
						
						bool bHighest = scoreManager.OverScore(mapManager.mapMode, mapManager.currentID);
						MessageCenter.Send(new MessageGameOver(
							mapManager.mapMode,
							mapManager.currentID,
							scoreManager.score,
							highHitCount,
							true, bHighest));
					}
				}
			}
		}
		public override void OnMessageRacketHit(MessageRacketHit msg) {}
		public override void OnMessageFallIntoTrap(MessageFallIntoTrap msg) {
			if (controller.ballAmount <= 1) {
				if (!_bOver) {
					_bOver = true;

					MessageCenter.Send(new MessageGameOver(
						mapManager.mapMode, mapManager.currentID,
						scoreManager.score, highHitCount, false, false));
					scoreManager.InitScore();
				}
			}
		}

		public override void OnStateCreate() {
			base.OnStateCreate();
		}
		public override void OnStateStart() {
			base.OnStateStart();

			var element = TableMapdat.instance.GetElement(mapManager.currentID);
			if (element) {
				foreach (var item in element.items) {
					if (controller.CreateBrick(item.id, item.hpMax, item.position)) {
						++_brickAmount;
					}
				}
			}
		}
		public override void OnStateDestroy() {
			base.OnStateDestroy();
		}
		public override void OnStateUpdate() {
			base.OnStateUpdate();
		}
	}
}