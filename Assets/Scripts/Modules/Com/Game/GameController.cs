using System.Collections.Generic;
using Game.Managers;
using Game.Tables;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(GameTouch))]
    public class GameController : MonoBehaviour {
        [SerializeField]
        private Transform _gameNode = null;
        private UnitRacket _currentRacket = null;
        private HashSet<UnitRacket> _handleRackets = new HashSet<UnitRacket>();
        private GameTouch _gameTouch = null;

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
                    _CreateBrick(item.id, item.position);
                }
                _CreateBall(Vector2.zero);
            }
        }

        private void OnDestroy() {
            RacketCache.instance.Clear();
            BallCache.instance.Clear();
            BrickCache.instance.Clear();
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
        private void _CreateBrick(int id, Vector2 pos) {
            BrickFactory.Create(id, pos, _gameNode);
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
    }
}