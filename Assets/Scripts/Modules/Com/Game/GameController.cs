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

        private void Awake() {
            _scriptManager = ManagerCenter.GetManager<ScriptManager>();
            _scriptManager.message.AddListener<PeakMessage_CreateEffect>(this, (PeakMessage_CreateEffect msg) => {
                _CreateEffect(msg.effectID, msg.position, msg.delay);
            });

            MessageCenter.AddListener<MessageBrickHit>(this, (MessageBrickHit msg) => {
                _scriptManager.ExecuteWithCache("effect_trigger", "brick_hit",
                    new ScriptValue(msg.uniqueID),
                    new ScriptValue(msg.position.x),
                    new ScriptValue(msg.position.y),
                    new ScriptValue(msg.attackID),
                    new ScriptValue((int) msg.attackElementType));

                if (msg.damageResult.bDie) {
                    _scriptManager.ExecuteWithCache("effect_trigger", "brick_die",
                        new ScriptValue(msg.uniqueID),
                        new ScriptValue(msg.position.x),
                        new ScriptValue(msg.position.y),
                        new ScriptValue(msg.attackID),
                        new ScriptValue((int) msg.attackElementType));
                }
            });
        }

        private void OnDestroy() {
            RacketCache.instance.Clear();
            BallCache.instance.Clear();
            BrickCache.instance.Clear();
            EffectCache.instance.Clear();
            MessageCenter.RemoveListener<MessageBrickHit>(this);
            _scriptManager.message.RemoveListener<PeakMessage_CreateEffect>(this);
        }

        private void Start() {
            _gameTouch = GetComponent<GameTouch>();
            _gameTouch.SetTouchBeginAction(_CreateRacket);
            _gameTouch.SetTouchDeltaAction(_SetRacketDirection);

            _InitGame();
        }

        private void _InitGame() {
            var mapManager = ManagerCenter.GetManager<MapManager>();
            var element = TableMapdat.instance.GetElement(mapManager.currentID);
            if (element) {
                foreach (var item in element.items) {
                    _CreateBrick(item.id, item.hpMax, item.position);
                }
                _CreateBall(Vector2.zero);
            }
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
        private void _CreateBall(Vector2 pos) {
            BallFactory.Create(1, pos, _gameNode, Vector2.one);
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
            if (delay <= float.Epsilon) {
                EffectFactory.Create(effectID, pos, _gameNode);
            } else {
                StartCoroutine(_DelayExecute(delay, () => {
                    EffectFactory.Create(effectID, pos, _gameNode);
                }));
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