using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitID))]
    [RequireComponent(typeof(UnitDestroy))]
    [RequireComponent(typeof(UnitTriggerMain))]
    public class UnitBall : MonoBehaviour {

        [SerializeField]
        private float _velocity = 0.1f;

        [SerializeField]
        private Vector2 _direction = Vector2.zero;

        private Vector3 _prevPosition = Vector3.zero;

        private UnitID _unitID = null;
        private UnitTriggerMain _unitCollider = null;
        private UnitDestroy _unitDestroy = null;

        private static List<System.Tuple<float, Vector2>> _dirList = new List<System.Tuple<float, Vector2>>() {
            new System.Tuple<float, Vector2>(0.393f, new Vector2(0.92f, 0.38f)), // 22.5
            new System.Tuple<float, Vector2>(0.785f, Vector2.one), // 45
            new System.Tuple<float, Vector2>(1.178f, new Vector2(0.38f, 0.92f)), // 67.5
            new System.Tuple<float, Vector2>(1.963f, new Vector2(-0.38f, 0.92f)), // 112.5
            new System.Tuple<float, Vector2>(2.355f, new Vector2(-1, 1)), // 135
            new System.Tuple<float, Vector2>(2.748f, new Vector2(-0.92f, 0.38f)), // 157.5
            new System.Tuple<float, Vector2>(-0.393f, new Vector2(0.92f, -0.38f)), // -22.5
            new System.Tuple<float, Vector2>(-0.785f, new Vector2(1, -1)), // -45
            new System.Tuple<float, Vector2>(-1.178f, new Vector2(0.38f, -0.92f)), // -67.5
            new System.Tuple<float, Vector2>(-1.963f, new Vector2(-0.38f, -0.92f)), // -112.5
            new System.Tuple<float, Vector2>(-2.355f, new Vector2(-1, -1)), // -135
            new System.Tuple<float, Vector2>(-2.748f, new Vector2(-0.92f, -0.38f)), // -157.5
        };

        public void SetVelocity(float v) {
            _velocity = v;
        }
        public void SetDirection(Vector2 dir) {
            _direction = dir;
        }

        private void Awake() {
            _unitID = GetComponent<UnitID>();
            _unitCollider = GetComponent<UnitTriggerMain>();
            _unitDestroy = GetComponent<UnitDestroy>();

            _unitCollider.triggerNotify.AddListener(this, (Collider2D other) => {
                var unitID = other.GetComponent<UnitID>();
                switch (unitID.elementType) {
                case ID_ElementType.Brick:
                    _OnTriggerRect(other, unitID);
                    break;
                case ID_ElementType.Racket:
                    _OnTriggerRacket(other, unitID);
                    break;
                case ID_ElementType.Edge:
                    _OnTriggerRect(other, unitID);
                    break;
                case ID_ElementType.Trap:
                    _OnTriggerTrap(other, unitID);
                    break;
                }

                MessageCenter.Send(new MessageBallCollision(_unitID.uniqueID, transform.position, other.ClosestPoint(transform.position)));

                transform.position = Vector3.Lerp(transform.position, _prevPosition, 0.5f);
            });
        }

        private void OnDestroy() {
            _unitCollider.triggerNotify.RemoveListener(this);
        }

        // Update is called once per frame
        void Update() {
            _prevPosition = transform.position;

            if (Vector2.zero == _direction) {
                return;
            }

            float deltaPosN = Time.deltaTime * _velocity;
            var radain = Mathf.Atan2(_direction.y, _direction.x);
            transform.Translate(Mathf.Cos(radain) * deltaPosN, Mathf.Sin(radain) * deltaPosN, 0);
        }

        private void _OnTriggerTrap(Collider2D target, UnitID targetID) {
            MessageCenter.Send(new MessageFallIntoTrap(_unitID.uniqueID));
            _unitDestroy.InvokeDestroy(DestroyType.None);
        }

        private void _OnTriggerRect(Collider2D target, UnitID targetID) {
            var targetHalfSize = new Vector2(target.bounds.size.x * 0.5f, target.bounds.size.y * 0.5f);
            Vector2 targetPos = target.transform.position;

            var point = target.ClosestPoint(transform.position);

            var dxL = point.x - (targetPos.x - targetHalfSize.x + target.offset.x);
            var dxR = point.x - (targetPos.x + targetHalfSize.x + target.offset.x);
            var dxB = point.y - (targetPos.y - targetHalfSize.y + target.offset.y);
            var dxT = point.y - (targetPos.y + targetHalfSize.y + target.offset.y);

            float[] tempArr = new float[] { Mathf.Abs(dxR), Mathf.Abs(dxB), Mathf.Abs(dxT) };
            // 0:left 1:right 2:bottom 3:top
            int minTag = 0;
            float min = dxL;
            for (var i = 0; i < tempArr.Length; ++i) {
                var value = tempArr[i];
                if (value < min) {
                    min = value;
                    minTag = i + 1;
                }
            }

            var previous = _direction;

            switch (minTag) {
            case 0:
                _direction.x = -Mathf.Abs(_direction.x);
                if (GCL.Base.Utility.PositiveNegativeNumber(previous.x) ==
                    GCL.Base.Utility.PositiveNegativeNumber(_direction.x)) {
                    _direction.y *= -1;
                }
                break;
            case 1:
                _direction.x = Mathf.Abs(_direction.x);
                if (GCL.Base.Utility.PositiveNegativeNumber(previous.x) ==
                    GCL.Base.Utility.PositiveNegativeNumber(_direction.x)) {
                    _direction.y *= -1;
                }
                break;
            case 2:
                _direction.y = -Mathf.Abs(_direction.y);
                if (GCL.Base.Utility.PositiveNegativeNumber(previous.y) ==
                    GCL.Base.Utility.PositiveNegativeNumber(_direction.y)) {
                    _direction.x *= -1;
                }
                break;
            case 3:
                _direction.y = Mathf.Abs(_direction.y);
                if (GCL.Base.Utility.PositiveNegativeNumber(previous.y) ==
                    GCL.Base.Utility.PositiveNegativeNumber(_direction.y)) {
                    _direction.x *= -1;
                }
                break;
            }

        }
        private void _OnTriggerRacket(Collider2D target, UnitID targetID) {
            var unitRacket = target.GetComponent<UnitRacket>();
            if (unitRacket.useDirection) {
                SetDirection(_CalcDirFromRacket(unitRacket.triggerDirection));
            } else {
                _direction.y = Mathf.Abs(_direction.y);
            }

            MessageCenter.Send(new MessageRacketHit(targetID.uniqueID, target.transform.position, _unitID.uniqueID));
        }

        private Vector2 _CalcDirFromRacket(Vector2 dir) {
            var radain = Mathf.Atan2(dir.y, dir.x);
            float v = 999.0f;
            var ret = Vector2.zero;
            foreach (var item in _dirList) {
                var c = Mathf.Abs(radain - item.Item1);
                if (c < v) {
                    v = c;
                    ret = item.Item2;
                }
            }
            return ret;
        }
    }
}