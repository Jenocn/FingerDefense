using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Modules {
    public class ActionBlink : MonoBehaviour {
        [SerializeField]
        private float _showInterval = 1;
        [SerializeField]
        private float _hideInterval = 1;
        [SerializeField]
        private bool _bChildren = false;

        public float showInterval => _showInterval;
        public float hideInterval => _hideInterval;

        private float _countTime = 0;
        private float _curInterval = 0;

        private bool _bShow = true;

        public void SetInterval(float showInterval_, float hideInterval_) {
            _showInterval = showInterval_;
            _hideInterval = hideInterval_;
        }

        private void Start() {
            _curInterval = showInterval;
        }

        private void Update() {
            _countTime += Time.deltaTime;
            if (_countTime >= _curInterval) {
                _bShow = !_bShow;
                _curInterval = _bShow ? showInterval : hideInterval;
                _countTime = 0;

                Utility.UnityUtility.ChangeVisible(transform, _bShow, _bChildren);
            }
        }
    }
}