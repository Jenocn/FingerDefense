using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(GameTouch))]
    [RequireComponent(typeof(StateMachine))]
    public class GameController : MonoBehaviour {
        [SerializeField]
        private Transform _gameNode = null;
        private StateMachine _stateMachine = null;
        private HashSet<UnitRacket> _handleRackets = new HashSet<UnitRacket>();
        private UnitRacket _currentRacket = null;

        public Transform gameNode { get => _gameNode; }
        public GameTouch gameTouch { get; private set; } = null;
        public int ballAmount { get; private set; } = 0;

        public void CreateRacket(Vector2 pos) {
            _handleRackets.RemoveWhere((UnitRacket item) => {
                if (item && item.gameObject.activeSelf) {
                    item.Destroy();
                }
                return !item || !item.gameObject.activeSelf;
            });
            _currentRacket = RacketFactory.Create(1, pos, _gameNode);
            _handleRackets.Add(_currentRacket);
        }
        public void CreateBall(int ballID, Vector2 pos, Vector2 direction, float delay) {
            ExecuteWithStartCoroutine(delay, () => {
                var ball = BallFactory.Create(ballID, pos, _gameNode, direction);
                var unitDestroy = ball.GetComponent<UnitDestroy>();
                ++ballAmount;
                unitDestroy.AddDestroyListener(this, (DestroyType t) => {
                    --ballAmount;
                });
            });
        }
        public bool CreateBrick(int id, int hpMax, Vector2 pos) {
            if (hpMax > 0) {
                var ret = BrickFactory.Create(id, hpMax, pos, _gameNode);
                return ret != null;
            }
            return false;
        }
        public void SetRacketDirection(Vector2 delta) {
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
        public void CreateEffect(int effectID, Vector2 pos, float delay) {
            ExecuteWithStartCoroutine(delay, () => {
                EffectFactory.Create(effectID, pos, _gameNode);
            });
        }

        public void ExecuteWithStartCoroutine(float delay, System.Action action) {
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

        private void Start() {
            ballAmount = 0;

            gameTouch = GetComponent<GameTouch>();
            _stateMachine = GetComponent<StateMachine>();

            var mapManager = ManagerCenter.GetManager<MapManager>();
            switch (mapManager.mapMode) {
            case MapMode.Classic:
                _stateMachine.ChangeState<GameStateClassic>();
                break;
            case MapMode.Infinite:
                _stateMachine.ChangeState<GameStateClassic>();
                break;
            case MapMode.Challenge:
                _stateMachine.ChangeState<GameStateClassic>();
                break;
            default:
                _stateMachine.ChangeState<GameStateClassic>();
                break;
            }
        }

        private void OnDestroy() {
            RacketCache.instance.Clear();
            BallCache.instance.Clear();
            BrickCache.instance.Clear();
            EffectCache.instance.Clear();
        }
    }
}