using UnityEngine;

namespace Game.Modules {
    public class GameTouch : MonoBehaviour {
        [SerializeField]
        private float _touchPercentY = 0.5f;
        [SerializeField]
        private float _delayTime = 0.5f;

        private Vector2 _touchDown = Vector2.zero;

        private bool _bTouchDown = false;
        private float _curDelay = 0;

        private System.Action<Vector2> _touchBeginAction = null;
        private System.Action<Vector2> _touchDeltaAction = null;

        public void SetTouchBeginAction(System.Action<Vector2> action) {
            _touchBeginAction = action;
        }
        public void SetTouchDeltaAction(System.Action<Vector2> action) {
            _touchDeltaAction = action;
        }

        // Update is called once per frame
        void Update() {
            if (_bTouchDown) {
                _curDelay += Time.deltaTime;
                if (_curDelay >= _delayTime) {
                    _bTouchDown = false;
                }
            }

            if (_bTouchDown) {
                var code = _TryGetTouchPositionUp(out var upRet);
                if (code == 1) {
                    _bTouchDown = false;
                    var touchDelta = upRet - _touchDown;
                    _InvokeTouchDelta(touchDelta);
                } else if (code == 2) {
                    _bTouchDown = false;
                }
            }

            if (!_bTouchDown) {
                if (_TryGetTouchPositionDown(out var downRet)) {
                    if (downRet.x >= 0 && downRet.x <= Screen.width) {
                        if (downRet.y >= 0 && downRet.y <= Screen.height * _touchPercentY) {
                            _touchDown = downRet;
                            _bTouchDown = true;
                            _curDelay = 0;
                            var pos = Camera.main.ScreenToWorldPoint(_touchDown);
                            _InvokeTouchBegin(pos);
                        }
                    }
                }
            }
        }

        private bool _TryGetTouchPositionDown(out Vector2 result) {
            result = Vector2.zero;
            if (Input.touchCount > 0) {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    result = touch.position;
                    return true;
                }
            } else if (Input.GetMouseButtonDown(0)) {
                result = Input.mousePosition;
                return true;
            }
            return false;
        }
        /// <summary>
        /// return: 0:false 1:true 2:true but result is invalid
        /// </summary>
        private int _TryGetTouchPositionUp(out Vector2 result) {
            result = Vector2.zero;
            if (Input.touchCount > 0) {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) {
                    result = touch.position;
                    return 1;
                } else if (touch.phase == TouchPhase.Ended) {
                    result = touch.position;
                    return 2;
                }
            } else if (Input.GetMouseButton(0)) {
                result = Input.mousePosition;
                return 1;
            }
            return 0;
        }

        private void _InvokeTouchBegin(Vector2 pos) {
            _touchBeginAction?.Invoke(pos);
        }
        private void _InvokeTouchDelta(Vector2 delta) {
            _touchDeltaAction?.Invoke(delta);
        }
    }
}