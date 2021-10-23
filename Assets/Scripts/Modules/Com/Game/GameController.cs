using System.Collections.Generic;
using Game.Managers;
using Game.Tables;
using GCL.Pattern;
using peak;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(GameTouch))]
    public class GameController : MonoBehaviour {
        [SerializeField]
        private Transform _gameNode = null;
        private UnitRacket _currentRacket = null;
        private HashSet<UnitRacket> _handleRackets = new HashSet<UnitRacket>();
        private GameTouch _gameTouch = null;
        private ScriptManager _scriptManager = null;
        private ScoreManager _scoreManager = null;

        private List<UnitBall> _unitBallList = new List<UnitBall>();

        private int _hitCount = 0;

        private void Awake() {
            _scriptManager = ManagerCenter.GetManager<ScriptManager>();
            _scoreManager = ManagerCenter.GetManager<ScoreManager>();

            _scriptManager.message.AddListener<PeakMessage_CreateEffect>(this, (PeakMessage_CreateEffect msg) => {
                _CreateEffect(msg.effectID, msg.position, msg.delay);
            });
            _scriptManager.message.AddListener<PeakMessage_CreateBall>(this, (PeakMessage_CreateBall msg) => {
                _CreateBall(msg.ballID, msg.position, msg.direction, msg.delay);
            });

            MessageCenter.AddListener<MessageBallCollision>(this, (MessageBallCollision msg) => {
                _scriptManager.ExecuteWithCache("trigger", "ball_collision",
                    new ScriptValue(msg.ballID),
                    new ScriptValue(msg.ballPosition.x),
                    new ScriptValue(msg.ballPosition.y),
                    new ScriptValue(msg.collisionPosition.x),
                    new ScriptValue(msg.collisionPosition.y));
            });

            MessageCenter.AddListener<MessageBrickHit>(this, (MessageBrickHit msg) => {

                _scriptManager.ExecuteWithCache("trigger", "brick_hit",
                    new ScriptValue(msg.uniqueID),
                    new ScriptValue(msg.position.x),
                    new ScriptValue(msg.position.y),
                    new ScriptValue(msg.attackID),
                    new ScriptValue((int) msg.attackElementType));

                if (msg.damageResult.bDie) {
                    _scriptManager.ExecuteWithCache("trigger", "brick_die",
                        new ScriptValue(msg.uniqueID),
                        new ScriptValue(msg.position.x),
                        new ScriptValue(msg.position.y),
                        new ScriptValue(msg.attackID),
                        new ScriptValue((int) msg.attackElementType));
                }

                ++_hitCount;

                Time.timeScale = 1 - Mathf.Clamp((_hitCount - 2) * 0.02f, 0, 0.3f);

                _scoreManager.AddScore(msg.uniqueID, msg.damageResult, _hitCount);
            });

            MessageCenter.AddListener<MessageRacketHit>(this, (MessageRacketHit msg) => {
                if (_unitBallList.Count <= 1) {
                    _hitCount = 0;
                    Time.timeScale = 1;
                }
            });
            MessageCenter.AddListener<MessageFallIntoTrap>(this, (MessageFallIntoTrap msg) => {
                if (_unitBallList.Count <= 1) {
                    _hitCount = 0;
                    Time.timeScale = 1;

                    MessageCenter.Send(new MessageGameFailed());
                }
            });
        }

        private void OnDestroy() {
            RacketCache.instance.Clear();
            BallCache.instance.Clear();
            BrickCache.instance.Clear();
            EffectCache.instance.Clear();

            MessageCenter.RemoveListener<MessageBallCollision>(this);
            MessageCenter.RemoveListener<MessageFallIntoTrap>(this);
            MessageCenter.RemoveListener<MessageRacketHit>(this);
            MessageCenter.RemoveListener<MessageBrickHit>(this);
            _scriptManager.message.RemoveListener<PeakMessage_CreateEffect>(this);
            _scriptManager.message.RemoveListener<PeakMessage_CreateBall>(this);
        }

        private void Start() {
            _gameTouch = GetComponent<GameTouch>();
            _gameTouch.SetTouchBeginAction((Vector2 pos) => {
                if (_unitBallList.Count > 0) {
                    _CreateRacket(pos);
                }
            });
            _gameTouch.SetTouchDeltaAction(_SetRacketDirection);

            _InitGame();
        }

        private void _InitGame() {
            _hitCount = 0;

            _scoreManager.InitScore();

            var mapManager = ManagerCenter.GetManager<MapManager>();
            var element = TableMapdat.instance.GetElement(mapManager.currentID);
            if (element) {
                foreach (var item in element.items) {
                    _CreateBrick(item.id, item.hpMax, item.position);
                }
                _CreateBall(1, Vector2.zero, Vector2.one, 1);
            }

            _scriptManager.ExecuteWithCache("trigger", "battle_start",
                new ScriptValue(mapManager.currentID));
        }

        private void _CreateRacket(Vector2 pos) {
            _handleRackets.RemoveWhere((UnitRacket item) => {
                if (item && item.gameObject.activeSelf) {
                    item.Destroy();
                }
                return !item || !item.gameObject.activeSelf;
            });
            _currentRacket = RacketFactory.Create(1, pos, _gameNode);
            _handleRackets.Add(_currentRacket);
        }
        private void _CreateBall(int ballID, Vector2 pos, Vector2 direction, float delay) {
            _ExecuteWithStartCoroutine(delay, () => {
                var ball = BallFactory.Create(ballID, pos, _gameNode, direction);
                var unitDestroy = ball.GetComponent<UnitDestroy>();
                unitDestroy.AddDestroyListener(this, (DestroyType dt) => {
                    _unitBallList.Remove(ball);
                });
                _unitBallList.Add(ball);
            });
        }
        private void _CreateBrick(int id, int hpMax, Vector2 pos) {
            if (hpMax > 0) {
                BrickFactory.Create(id, hpMax, pos, _gameNode);
            }
        }
        private void _SetRacketDirection(Vector2 delta) {
            if (_currentRacket) {
                if (delta != Vector2.zero) {
                    if (delta.y < 0) {
                        delta.y *= -1;
                    }
                    _currentRacket.SetDirection(delta.normalized);
                    _currentRacket = null;
                }
            }
        }
        private void _CreateEffect(int effectID, Vector2 pos, float delay) {
            _ExecuteWithStartCoroutine(delay, () => {
                EffectFactory.Create(effectID, pos, _gameNode);
            });
        }

        private void _ExecuteWithStartCoroutine(float delay, System.Action action) {
            if (action == null) {
                return;
            }
            if (delay <= float.Epsilon) {
                action();
            } else {
                StartCoroutine(_DelayExecute(delay, action));
            }
        }

        private System.Collections.IEnumerator _DelayExecute(float delay, System.Action action) {
            yield return new WaitForSeconds(delay);
            if (action != null) {
                action.Invoke();
            }
        }
    }
}