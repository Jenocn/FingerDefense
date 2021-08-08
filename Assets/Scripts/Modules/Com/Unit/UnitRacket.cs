using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitRacket : MonoBehaviour {

        [SerializeField]
        private float _holdSec = 0.5f;

        private Vector2 _direction = Vector2.up;
        public Vector2 triggerDirection => _direction;

        private float _curSec = 0;

        private UnitDestroy _unitDestroy = null;
        private SpriteRenderer _spriteRenderer = null;
        private bool _bDestroy = false;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _unitDestroy = GetComponent<UnitDestroy>();
        }

        public void OnBallTrigger() {
            Destroy();
        }

        public void SetDirection(Vector2 dir) {
            _direction = dir;
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