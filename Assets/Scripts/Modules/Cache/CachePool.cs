using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    public class CachePool : Singleton<CachePool> {
        private LinkedList<GameObject> _pool = new LinkedList<GameObject>();
        public GameObject Pop() {
            if (_pool.Last != null) {
                var ret = _pool.Last.Value;
                _pool.RemoveLast();
                ret.SetActive(true);
                return ret;
            }
            return null;
        }
        public void Push(GameObject gameObject) {
            gameObject.SetActive(false);
            _pool.AddFirst(gameObject);
        }
        public void Clear(System.Action<GameObject> action) {
            foreach (var item in _pool) {
                action.Invoke(item);
            }
            _pool.Clear();
        }
        public int Size() {
            return _pool.Count;
        }
        public bool Empty() {
            return _pool.Last == null;
        }
        public bool Contains(GameObject gameObject) {
            return _pool.Find(gameObject) != null;
        }
        public GameObject Find(System.Func<GameObject, bool> func) {
            foreach (var item in _pool) {
                if (func.Invoke(item)) {
                    return item;
                }
            }
            return null;
        }
    }
}