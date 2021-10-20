using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(Renderer))]
    public class ActionBlink : MonoBehaviour {
        [SerializeField]
        private float _showInterval = 1;
        [SerializeField]
        private float _hideInterval = 1;
        private Renderer _renderer = null;

        public float showInterval => _showInterval;
        public float hideInterval => _hideInterval;

        private float _countTime = 0;
        private float _curInterval = 0;

        public void SetInterval(float showInterval_, float hideInterval_) {
            _showInterval = showInterval_;
            _hideInterval = hideInterval_;
        }

        private void Start() {
            _renderer = GetComponent<Renderer>();
            if (_renderer) {
                _curInterval = _renderer.enabled ? showInterval : hideInterval;
            }
        }

        private void Update() {
            if (_renderer) {
                _countTime += Time.deltaTime;
                if (_countTime >= _curInterval) {
                    _renderer.enabled = !_renderer.enabled;
                    _curInterval = _renderer.enabled ? showInterval : hideInterval;
                    _countTime = 0;
                }
            }
        }
    }
}