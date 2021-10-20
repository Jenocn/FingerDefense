using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    public class ActionFollow : MonoBehaviour {
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        private float _velocityRate = 30;

        [SerializeField]
        private float _distance = 0;

        public Transform target => _target;
        public float velocityRate => _velocityRate;
        public float distance => _distance;

        public void SetTarget(Transform target) {
            _target = target;
        }
        public void SetVelocityRate(float value) {
            _velocityRate = value;
        }
        public void SetDistance(float value) {
            _distance = value;
        }

        private void LateUpdate() {
            if (_target) {
                var dis = Vector3.Distance(transform.position, _target.position);
                if (dis > _distance) {
                    var delta = _target.position - transform.position;
                    var pos = _target.position - delta.normalized * _distance;
                    transform.position = Vector3.Lerp(transform.position, pos, Mathf.Clamp01(_velocityRate * Time.deltaTime));
                }
            }
        }
    }
}