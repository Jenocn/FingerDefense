using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitBomb : MonoBehaviour {
        private CircleCollider2D _collider = null;
        private float _radius = 0;
        private void Awake() {
            _collider = GetComponent<CircleCollider2D>();
            _radius = _collider.radius;

            Init();
        }

        private void Update() {
            _collider.radius = _radius * transform.localScale.x;
        }

        public void Init() {
            transform.localScale = Vector3.zero;
            _collider.radius = 0;

            transform.DOScale(1.2f, 0.5f).OnComplete(() => {
                GetComponent<UnitDestroy>().InvokeDestroy(DestroyType.None);
            });
        }
    }
}