using Game.Tables;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public class GameStateClassic : GameStateBase {
		public override void OnMessageBrickHit(MessageBrickHit msg) {
			scoreManager.AddScore(mapManager.mapMode, msg.uniqueID, msg.damageResult, hitCount);
		}
		public override void OnMessageRacketHit(MessageRacketHit msg) {}
		public override void OnMessageFallIntoTrap(MessageFallIntoTrap msg) {}

		public override void OnGameControllerEvent(GameController.Event e) {
			if (e == GameController.Event.BrickZero) {
				controller.Stop();

				bool bHighest = scoreManager.OverScore(mapManager.mapMode, mapManager.currentID);
				mapManager.SetNextMapClassicLocked(mapManager.currentID, false);
				MessageCenter.Send(new MessageGameOver(
					mapManager.mapMode,
					mapManager.currentID,
					scoreManager.score,
					highHitCount,
					true, bHighest));
			} else if (e == GameController.Event.BallZero) {
				controller.Stop();
				MessageCenter.Send(new MessageGameOver(
					mapManager.mapMode, mapManager.currentID,
					scoreManager.score, highHitCount, false, false));
			}
		}
		public override void OnGamePause(bool bPause) {}

		public override void OnStateCreate() {
			base.OnStateCreate();
		}
		public override void OnStateStart() {
			base.OnStateStart();

			var element = TableMapdat.instance.GetElement(mapManager.currentID);
			if (element) {
				var i = 0;
				foreach (var item in element.items) {
					controller.ExecuteWithStartCoroutine(i * 0.003f, () => {
						controller.CreateBrick(item.id, item.hpMax, item.position);
					});
					++i;
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