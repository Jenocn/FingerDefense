using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSequenceAnimation : MonoBehaviour {

        [SerializeField]
        private List<Sprite> _sprites = null;
        [SerializeField, Range(0.1f, 60)]
        private float _fps = 10;
        [SerializeField]
        private bool _loop = false;
        [SerializeField]
        private bool _playOnStart = true;

        private SpriteRenderer _spriteRenderer = null;
        private SimpleNotify<int> _simpleFrameNotify = new SimpleNotify<int>();
        private SimpleNotifyVoid _simpleEndNotify = new SimpleNotifyVoid();
        private int _index = 0;
        private float _timeCount = 0;
        private float _timeDuration = 0;
        private bool _running = false;

        public int index { get => _index; }
        public int size { get => _sprites.Count; }

        void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _timeDuration = 1.0f / Mathf.Max(0.1f, _fps);
        }

        void Start() {
            if (_playOnStart) {
                Play();
            } else {
                Stop();
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_running) {
                return;
            }

            _timeCount += Time.deltaTime;
            if (_timeCount >= _timeDuration) {
                while (_timeCount >= _timeDuration) {
                    _timeCount -= _timeDuration;
                    ++_index;

                    if (_index >= size) {
                        _index = 0;
                        if (!_loop) {
                            Stop();
                            return;
                        }
                    }
                }
                _spriteRenderer.sprite = _sprites[_index];
                _simpleFrameNotify.Send(_index);
            }
        }

        public void Stop() {
            gameObject.SetActive(false);
            _running = false;
            _simpleEndNotify.Send();
        }
        public void Play() {
            gameObject.SetActive(true);
            if (_index >= 0 && _index < size) {
                _spriteRenderer.sprite = _sprites[_index];
                _running = true;
                _simpleFrameNotify.Send(_index);
            } else {
                _running = false;
            }
        }
        public void Pause() {
            _running = false;
        }

        public void AddEndCallback(object sender, System.Action action) {
            _simpleEndNotify.AddListener(sender, action);
        }
        public void RemoveEndCallback(object sender) {
            _simpleEndNotify.RemoveListener(sender);
        }
        public void AddFrameCallback(object sender, System.Action<int> action) {
            _simpleFrameNotify.AddListener(sender, action);
        }
        public void RemoveFrameCallback(object sender) {
            _simpleFrameNotify.RemoveListener(sender);

        }
    }
}