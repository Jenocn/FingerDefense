using System.Collections.Generic;
using Game.Managers;
using Game.Views;
using GCL.Pattern;
using UnityEngine;
using UnityUiModel;

namespace Game.Modules {
    [RequireComponent(typeof(GameTouch))]
    [RequireComponent(typeof(UiStack))]
    [RequireComponent(typeof(StateMachine))]
    public class GameController : MonoBehaviour {

        public enum Event {
            BallZero,
            BrickZero,
        }

        [SerializeField]
        private Transform _gameNode = null;
        private StateMachine _stateMachine = null;
        private HashSet<UnitRacket> _handleRackets = new HashSet<UnitRacket>();
        private UnitRacket _currentRacket = null;
        private LinkedList<UnitBall> _unitBalls = new LinkedList<UnitBall>();

        public Transform gameNode { get => _gameNode; }
        public UiStack uiStack { get; private set; } = null;
        public GameTouch gameTouch { get; private set; } = null;
        public int ballAmount { get; private set; } = 0;
        public int brickAmount { get; private set; } = 0;

        public SimpleNotify<Event> events { get; private set; } = new SimpleNotify<Event>();

        public bool isStop { get; private set; } = false;

        public void Stop() {
            isStop = true;
            gameTouch.enabled = false;
            foreach (var item in _unitBalls) {
                PauseBall(item, true);
            }
        }

        public void Run() {
            isStop = false;
            gameTouch.enabled = true;
            foreach (var item in _unitBalls) {
                PauseBall(item, false);
            }
        }

        public void PauseBall(UnitBall ball, bool bPause) {
            ball.SetMoveEnabled(!bPause);
            ball.gameObject.SetActive(!bPause);
        }

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
                if (ball) {
                    var unitDestroy = ball.GetComponent<UnitDestroy>();
                    ++ballAmount;
                    _unitBalls.AddLast(ball);
                    unitDestroy.AddDestroyListener(this, (DestroyType t) => {
                        --ballAmount;
                        _unitBalls.Remove(ball);

                        if (ballAmount == 0) {
                            events.Send(Event.BallZero);
                        }
                    });
                    if (isStop) {
                        PauseBall(ball, true);
                    }
                }
            });
        }
        public bool CreateBrick(int id, int hpMax, Vector2 pos, System.Action destroyAction = null) {
            if (hpMax > 0) {
                var ret = BrickFactory.Create(id, hpMax, pos, _gameNode);
                if (ret) {
                    var unitDestroy = ret.GetComponent<UnitDestroy>();
                    ++brickAmount;
                    unitDestroy.AddDestroyListener(this, (DestroyType t) => {
                        --brickAmount;

                        if (brickAmount == 0) {
                            events.Send(Event.BrickZero);
                        }
                        if (destroyAction != null) {
                            destroyAction();
                        }
                    });
                    return true;
                }
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

        public void CountDown(int time, System.Action action) {
            var uiCountDown = uiStack.PushUI<UiCountDown>();
            uiCountDown.Play(time, action);
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
            uiStack = GetComponent<UiStack>();
            _stateMachine = GetComponent<StateMachine>();

            var mapManager = ManagerCenter.GetManager<MapManager>();
            switch (mapManager.mapMode) {
            case MapMode.Classic:
                _stateMachine.ChangeState<GameStateClassic>();
                break;
            case MapMode.Infinite:
                _stateMachine.ChangeState<GameStateInfinite>();
                break;
            case MapMode.Challenge:
                _stateMachine.ChangeState<GameStateChallenge>();
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