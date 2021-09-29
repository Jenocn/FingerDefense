using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    [RequireComponent(typeof(UnitTriggerSub))]
    public class UnitRacket : MonoBehaviour {

        [SerializeField]
        private float _holdSec = 0.5f;

        private Vector2 _direction = Vector2.up;
        public Vector2 triggerDirection => _direction;

        private float _curSec = 0;

        private UnitDestroy _unitDestroy = null;
        private UnitTriggerSub _unitTriggerSub = null;
        private SpriteRenderer _spriteRenderer = null;
        private bool _bDestroy = false;
        private bool _bUseDirection = false;
        public bool useDirection { get => _bUseDirection; }

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _unitDestroy = GetComponent<UnitDestroy>();
            _unitTriggerSub = GetComponent<UnitTriggerSub>();

            _unitTriggerSub.triggerNotify.AddListener(this, (Collider2D other) => {
                Destroy();
            });
        }

        private void OnDestroy() {
            _unitTriggerSub.triggerNotify.RemoveListener(this);
        }

        public void ResetDirection() {
            _bUseDirection = false;
            _direction = Vector2.up;
        }

        public void SetDirection(Vector2 dir) {
            _direction = dir;
            _bUseDirection = true;
        }

        public void Init() {
            _curSec = 0;
            _spriteRenderer.color = Color.white;
            _bDestroy = false;
        }

        public void Destroy() {
            if (!_bDestroy) {
                _spriteRenderer.DOFade(0, 0.3f).OnComplete(() => {
                    _unitDestroy.InvokeDestroy(DestroyType.None);
                });
                _bDestroy = true;
            }
        }

        // Update is called once per frame
        void Update() {
            if (gameObject.activeSelf) {
                _curSec += Time.deltaTime;
                if (_curSec >= _holdSec) {
                    Destroy();
                }
            }
        }
    }
}