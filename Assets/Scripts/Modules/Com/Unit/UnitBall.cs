using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Modules {

    [RequireComponent(typeof(UnitDestroy))]
    public class UnitBall : MonoBehaviour {

        [SerializeField]
        private float _velocity = 0.1f;

        [SerializeField]
        private Vector2 _dir = Vector2.zero;
        [SerializeField]
        private AttackData _attackData = new AttackData();
        public AttackData attackData { get => _attackData; }

        private Vector2 _halfSize = Vector2.zero;
        private BoxCollider2D _collider = null;
        private Vector3 _prevPosition = Vector3.zero;

        private UnitDestroy _unitDestroy = null;

        public void SetVelocity(float v) {
            _velocity = v;
        }
        public void SetDirection(Vector2 dir) {
            _dir = dir;
        }
        public void SetDirection(float x, float y) {
            _dir.Set(x, y);
        }

        private void Awake() {
            _unitDestroy = GetComponent<UnitDestroy>();
        }

        // Start is called before the first frame update
        void Start() {
            _collider = GetComponent<BoxCollider2D>();
            _halfSize.Set(_collider.bounds.size.x * 0.5f, _collider.bounds.size.y * 0.5f);
        }

        // Update is called once per frame
        void Update() {
            _prevPosition = transform.position;

            if (Vector2.zero == _dir) {
                return;
            }

            float deltaPosN = Time.deltaTime * _velocity;
            var radain = Mathf.Atan2(_dir.y, _dir.x);
            transform.Translate(Mathf.Cos(radain) * deltaPosN, Mathf.Sin(radain) * deltaPosN, 0);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var unitID = other.GetComponent<UnitID>();
            if (!unitID) {
                return;
            }
            switch (unitID.elementType) {
            case ElementType.Brick:
                _OnTriggerBrick(other);
                break;
            case ElementType.Racket:
                _OnTriggerRacket(other);
                break;
            case ElementType.Edge:
                _OnTriggerEdge(other);
                break;
            }
            transform.position = Vector3.Lerp(transform.position, _prevPosition, 0.5f);
        }

        private void _OnTriggerEdge(Collider2D target) {
            var unitEdge = target.GetComponent<UnitEdge>();
            var dir = unitEdge.direction.normalized;
            var l = Mathf.Abs(Vector2.Dot(_dir.normalized, dir));
            var fdir = (_dir.normalized + dir * l * 2).normalized;
            SetDirection(fdir);
        }

        private void _OnTriggerBrick(Collider2D target) {
            var targetHalfSize = new Vector2(target.bounds.size.x * 0.5f, target.bounds.size.y * 0.5f);
            Vector2 targetPos = target.transform.position;

            var point = target.ClosestPoint(transform.position);
            var dxL = point.x - (targetPos.x - targetHalfSize.x);
            var dxR = point.x - (targetPos.x + targetHalfSize.x);
            var dxB = point.y - (targetPos.y - targetHalfSize.y);
            var dxT = point.y - (targetPos.y + targetHalfSize.y);

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

            switch (minTag) {
            case 0:
                if (_dir.x > 0) {
                    _dir.x *= -1;
                } else {
                    _dir.y *= -1;
                }
                break;
            case 1:
                if (_dir.x < 0) {
                    _dir.x *= -1;
                } else {
                    _dir.y *= -1;
                }
                break;
            case 2:
                if (_dir.y > 0) {
                    _dir.y *= -1;
                } else {
                    _dir.x *= -1;
                }
                break;
            case 3:
                if (_dir.y < 0) {
                    _dir.y *= -1;
                } else {
                    _dir.x *= -1;
                }
                break;
            }

            // notice brick
            var unitBrick = target.GetComponent<UnitBrick>();
            unitBrick.OnDamage(attackData);
        }
        private void _OnTriggerRacket(Collider2D target) {
            var unitRacket = target.GetComponent<UnitRacket>();
            unitRacket.OnBallTrigger();
            var tempX = Mathf.Lerp(unitRacket.triggerDirection.x, _dir.x, 0.2f);
            var tempY = unitRacket.triggerDirection.y;
            if (tempY == 0) {
                tempY = 0.1f;
            }
            SetDirection(tempX, tempY);
        }
    }
}