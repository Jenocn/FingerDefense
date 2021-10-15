using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSequenceAnimation : MonoBehaviour {

        private static LogSystem.Logger _logger = null;

        static SpriteSequenceAnimation() {
            _logger = LogSystem.GetLogger("SpriteSequenceAnimation");
        }

        [System.Serializable]
        public class FrameData {
            public Sprite sprite = null;
            public int frame = 1;
        }
        public class FrameIndex {
            public FrameIndex(int main, int sub, int frame) {
                mainIndex = main;
                subIndex = sub;
                frameIndex = frame;
            }
            public int mainIndex { get; private set; } = 0;
            public int subIndex { get; private set; } = 0;
            public int frameIndex { get; private set; } = 0;

            public void Log() {
                _logger.Log("main: " + mainIndex + ", sub: " + subIndex + ", frame: " + frameIndex);
            }
        }

        [SerializeField]
        private List<FrameData> _frames = null;
        private List<System.Tuple<Sprite, FrameIndex>> _sprites = new List<System.Tuple<Sprite, FrameIndex>>();
        [SerializeField, Range(0.1f, 60)]
        private float _fps = 10;
        [SerializeField]
        private bool _loop = false;
        [SerializeField]
        private bool _playOnStart = true;

        private SpriteRenderer _spriteRenderer = null;
        private SimpleNotify<FrameIndex> _simpleFrameNotify = new SimpleNotify<FrameIndex>();
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
            _InstallSprites();
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
                var curSprite = _sprites[_index];
                _spriteRenderer.sprite = curSprite.Item1;
                _simpleFrameNotify.Send(curSprite.Item2);
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
                var curSprite = _sprites[_index];
                _spriteRenderer.sprite = curSprite.Item1;
                _running = true;
                _simpleFrameNotify.Send(curSprite.Item2);
            } else {
                _running = false;
            }
        }
        public void Replay() {
            _index = 0;
            Play();
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
        public void AddFrameCallback(object sender, System.Action<FrameIndex> action) {
            _simpleFrameNotify.AddListener(sender, action);
        }
        public void RemoveFrameCallback(object sender) {
            _simpleFrameNotify.RemoveListener(sender);
        }

        private void _InstallSprites() {
            _sprites.Clear();
            for (int i = 0; i < _frames.Count; ++i) {
                var item = _frames[i];
                for (int j = 0; j < item.frame; ++j) {
                    _sprites.Add(new System.Tuple<Sprite, FrameIndex>(item.sprite, new FrameIndex(i, j, _sprites.Count)));
                }
            }
        }
    }
}