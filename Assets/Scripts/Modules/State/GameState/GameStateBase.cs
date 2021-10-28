using Game.Managers;
using Game.Views;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
	public abstract class GameStateBase : StateBase {
		protected GameController controller { get; private set; } = null;
		protected ScriptManager scriptManager { get; private set; } = null;
		protected ScoreManager scoreManager { get; private set; } = null;
		protected MapManager mapManager { get; private set; } = null;
		protected peak.VirtualJourney scriptTrigger { get; private set; } = null;

		protected int highHitCount { get; set; } = 0;
		protected int hitCount { get; set; } = 0;

		public abstract void OnMessageBrickHit(MessageBrickHit msg);
		public abstract void OnMessageRacketHit(MessageRacketHit msg);
		public abstract void OnMessageFallIntoTrap(MessageFallIntoTrap msg);
		public abstract void OnGameControllerEvent(GameController.Event e);
		public abstract void OnGamePause(bool bPause);

		public override void OnStateCreate() {
			controller = GetComponent<GameController>();
			scriptManager = ManagerCenter.GetManager<ScriptManager>();
			scoreManager = ManagerCenter.GetManager<ScoreManager>();
			mapManager = ManagerCenter.GetManager<MapManager>();
			scriptTrigger = scriptManager.LoadWithCache("trigger.peak");

			controller.gameTouch.SetTouchBeginAction((Vector2 pos) => {
				controller.CreateRacket(pos);
			});
			controller.gameTouch.SetTouchDeltaAction((Vector2 pos) => {
				controller.SetRacketDirection(pos);
			});

			controller.events.AddListener(this, OnGameControllerEvent);

			scriptManager.message.AddListener<PeakMessage_CreateEffect>(this, (PeakMessage_CreateEffect msg) => {
				controller.CreateEffect(msg.effectID, msg.position, msg.delay);
			});
			scriptManager.message.AddListener<PeakMessage_CreateBall>(this, (PeakMessage_CreateBall msg) => {
				controller.CreateBall(msg.ballID, msg.position, msg.direction, msg.delay);
			});
			scriptManager.message.AddListener<PeakMessage_CreateBrick>(this, (PeakMessage_CreateBrick msg) => {
				controller.CreateBrick(msg.brickID, msg.hpMax, msg.position);
			});

			MessageCenter.AddListener<MessageBallCollision>(this, (MessageBallCollision msg) => {
				scriptTrigger.ExecuteFunction("ball_collision",
					new peak.interpreter.ValueNumber(msg.ballID),
					new peak.interpreter.ValueNumber(msg.ballPosition.x),
					new peak.interpreter.ValueNumber(msg.ballPosition.y),
					new peak.interpreter.ValueNumber(msg.collisionPosition.x),
					new peak.interpreter.ValueNumber(msg.collisionPosition.y));
			});

			MessageCenter.AddListener<MessageBrickHit>(this, (MessageBrickHit msg) => {

				scriptTrigger.ExecuteFunction("brick_hit",
					new peak.interpreter.ValueNumber(msg.uniqueID),
					new peak.interpreter.ValueNumber(msg.position.x),
					new peak.interpreter.ValueNumber(msg.position.y),
					new peak.interpreter.ValueNumber(msg.attackID),
					new peak.interpreter.ValueNumber((int) msg.attackElementType));

				if (msg.damageResult.bDie) {
					scriptTrigger.ExecuteFunction("brick_die",
						new peak.interpreter.ValueNumber(msg.uniqueID),
						new peak.interpreter.ValueNumber(msg.position.x),
						new peak.interpreter.ValueNumber(msg.position.y),
						new peak.interpreter.ValueNumber(msg.attackID),
						new peak.interpreter.ValueNumber((int) msg.attackElementType));
				}

				++hitCount;
				highHitCount = Mathf.Max(highHitCount, hitCount);

				OnMessageBrickHit(msg);
			});

			MessageCenter.AddListener<MessageRacketHit>(this, (MessageRacketHit msg) => {
				if (controller.ballAmount <= 1) {
					hitCount = 0;
				}

				OnMessageRacketHit(msg);
			});
			MessageCenter.AddListener<MessageFallIntoTrap>(this, (MessageFallIntoTrap msg) => {
				if (controller.ballAmount <= 1) {
					hitCount = 0;
				}
				OnMessageFallIntoTrap(msg);
			});

			MessageCenter.AddListener<MessageGamePause>(this, (MessageGamePause msg) => {
				if (msg.bPause) {
					controller.Stop();
					controller.uiStack.PushUI<UiGamePause>();
				} else {
					controller.Run();
				}
				OnGamePause(msg.bPause);
			});

			hitCount = 0;
			scoreManager.InitScore();
		}

		public override void OnStateStart() {
			scriptManager.Execute(scriptTrigger, "battle_start",
				new ScriptValue((int) mapManager.mapMode),
				new ScriptValue(mapManager.currentID));

			controller.CountDown(3, () => {
				controller.CreateBall(1, new Vector2(0, -2), Vector2.zero, 0);
			});
		}

		public override void OnStateDestroy() {
			MessageCenter.RemoveListener<MessageBallCollision>(this);
			MessageCenter.RemoveListener<MessageFallIntoTrap>(this);
			MessageCenter.RemoveListener<MessageRacketHit>(this);
			MessageCenter.RemoveListener<MessageBrickHit>(this);
			MessageCenter.RemoveListener<MessageGamePause>(this);
			scriptManager.message.RemoveListener<PeakMessage_CreateEffect>(this);
			scriptManager.message.RemoveListener<PeakMessage_CreateBall>(this);
			scriptManager.message.RemoveListener<PeakMessage_CreateBrick>(this);
			controller.events.RemoveListener(this);
		}

	}
}