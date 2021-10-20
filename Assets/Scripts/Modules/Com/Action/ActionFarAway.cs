using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    public class ActionFarAway : MonoBehaviour {

        [System.Serializable]
        public class Element {
            public Transform target = null;
            public float velocityRate = 30;
            public float distance = 1;
        }

        [SerializeField]
        private List<Element> _elements = new List<Element>();

        public List<Element> elements => _elements;

        public void AddTarget(Transform transform, float velocityRate, float distance) {
            if (!ContainsTarget(transform)) {
                var e = new Element();
                e.target = transform;
                e.velocityRate = velocityRate;
                e.distance = distance;
                _elements.Add(e);
            }
        }
        public void RemoveTarget(Transform transform) {
            var ite = _elements.Find((Element e) => {
                return e.target == transform;
            });
            _elements.Remove(ite);
        }
        public bool ContainsTarget(Transform transform) {
            return (null != _elements.Find((Element e) => {
                return e.target == transform;
            }));
        }
        public void Clear() {
            _elements.Clear();
        }

        private void LateUpdate() {
            foreach (var element in elements) {
                var target = element.target;
                if (target) {
                    var dis = Vector3.Distance(transform.position, target.position);
                    if (dis <= element.distance) {
                        var delta = target.position - transform.position;
                        var pos = target.position - delta.normalized * element.distance;
                        transform.position = Vector3.Lerp(transform.position, pos, Mathf.Clamp01(element.velocityRate * Time.deltaTime));
                    }
                }
            }
        }
    }
}