using System.Collections.Generic;
using GCL.Base;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(BoxCollider2D))]
    public class BackgroundDecorateSpawn : MonoBehaviour {
        [SerializeField]
        private GameObject[] _prefabs = null;
        [SerializeField]
        private float _duration = 1;
        [SerializeField]
        private int _multiplyMin = 1;
        [SerializeField]
        private int _multiplyMax = 1;
        [SerializeField]
        private bool _spawnOnCamera = true;
        [SerializeField, Range(0, 1)]
        private float _percent = 1;

        private float _timeCount = 0;

        private BoxCollider2D _collider = null;
        private Dictionary<int, CachePool> _cachePool = new Dictionary<int, CachePool>();

        private static Rect _rectScreen;
        private static Vector2 _tempPosition = new Vector2();

        private void Start() {
            _rectScreen.Set(-100, -100, Screen.width + 200, Screen.height + 200);
            _collider = GetComponent<BoxCollider2D>();
            if (_prefabs.Length == 0) {
                enabled = false;
            }
        }

        private void Update() {
            _timeCount += Time.deltaTime;
            if (_timeCount >= _duration) {
                _timeCount = 0;

                if (RandomTool.HitWithPercent(_percent)) {
                    var count = Random.Range(_multiplyMin, _multiplyMax + 1);
                    for (int i = 0; i < count; ++i) {
                        _GenerateObject();
                    }
                }
            }
        }

        private void _GenerateObject() {
            if (_spawnOnCamera) {
                var worldPos = transform.TransformPoint(Vector3.zero);
                var cam = Camera.main;
                var minPos = worldPos + cam.ScreenToWorldPoint(_rectScreen.min);
                var maxPos = worldPos + cam.ScreenToWorldPoint(_rectScreen.max);
                _tempPosition = transform.InverseTransformPoint(
                    Random.Range(minPos.x, maxPos.x),
                    Random.Range(minPos.y, maxPos.y),
                    0
                );
            } else {
                _tempPosition.Set(
                    Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
                    Random.Range(_collider.bounds.min.y, _collider.bounds.max.y)
                );
            }

            var index = Random.Range(0, _prefabs.Length);
            var obj = _GetPool(index).Pop();
            if (!obj) {
                obj = Instantiate(_prefabs[index], transform);
            }
            obj.transform.position = _tempPosition;

            var animation = obj.GetComponent<SpriteSequenceAnimation>();
            if (animation) {
                animation.Play();
                animation.AddEndCallback(this, () => {
                    _GetPool(index).Push(obj);
                });
            }
        }

        private CachePool _GetPool(int index) {
            if (!_cachePool.TryGetValue(index, out var pool)) {
                pool = new CachePool();
                _cachePool.Add(index, pool);
            }
            return pool;
        }
    }
}