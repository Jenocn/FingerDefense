using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Modules {
    [RequireComponent(typeof(Button))]
    public class StateButton : MonoBehaviour {
        [System.Serializable]
        public class State {
            public Sprite image = null;
            public UnityEvent onClick = null;
        }

        [SerializeField]
        private List<State> _states = null;

        private Button _button = null;
        private int _index = 0;
        private State _currentState = null;
        private System.Action<int> _forceAction = null;

        private void Awake() {
            _button = GetComponent<Button>();
        }

        private void Start() {
            _UpdateState();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(Invoke);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            var button = GetComponent<Button>();
            if (button.onClick.GetPersistentEventCount() > 0) {
                LogSystem.defaultLogger.LogError("‘StateButton’ will take over the ‘onClick’ event of ’Button‘");
            }
        }
#endif

        public State GetState(int stateIndex) {
            if (stateIndex >= 0 && stateIndex < _states.Count) {
                return _states[stateIndex];
            }
            return null;
        }

        /// <summary>
        /// 供代码调用的回调函数,函数中的int参数为回调时的状态index
        /// </summary>
        public void SetForceAction(System.Action<int> action) {
            _forceAction = action;
        }
        public void RemoveForceAction() {
            _forceAction = null;
        }

        public void AddListener(int stateIndex, UnityAction e) {
            if (stateIndex >= 0 && stateIndex < _states.Count) {
                _states[stateIndex].onClick.AddListener(e);
            }
        }

        public void RemoveListener(int stateIndex, UnityAction e) {
            if (stateIndex >= 0 && stateIndex < _states.Count) {
                _states[stateIndex].onClick.RemoveListener(e);
            }
        }

        public void RemoveAllListeners(int stateIndex) {
            if (stateIndex >= 0 && stateIndex < _states.Count) {
                _states[stateIndex].onClick.RemoveAllListeners();
            }
        }

        public void Invoke() {
            _forceAction?.Invoke(_index);
            _currentState?.onClick?.Invoke();
            _UpdateState();
        }

        private void _UpdateState() {
            if (_states == null || _states.Count == 0) {
                return;
            }

            if (_currentState == null) {
                _index = 0;
            } else {
                ++_index;
                if (_index >= _states.Count) {
                    _index = 0;
                }
            }
            _currentState = _states[_index];
            if (_currentState.image) {
                _button.image.sprite = _currentState.image;
            }
        }
    }
}