using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    public class UnitDestroy : MonoBehaviour {

        private Dictionary<int, System.Action<DestroyType>> _destroyListeners = new Dictionary<int, System.Action<DestroyType>>();
        private HashSet<int> _removeList = new HashSet<int>();
        private System.Action<DestroyType> _actionDestroy = null;

        public void AddDestroyListener(object sender, System.Action<DestroyType> action) {
            int hashCode = sender.GetHashCode();
            _destroyListeners[hashCode] = action;
            _removeList.Remove(hashCode);
        }
        public void RemoveDestroyListener(object sender) {
            _removeList.Add(sender.GetHashCode());
        }

        public void ResetDestroyFunc(System.Action<DestroyType> action) {
            _actionDestroy = action;
        }
        public void InvokeDestroy(DestroyType type, float delaySec = 0) {
            if (delaySec > 0) {
                StartCoroutine(_StartDestroy(type, delaySec));
            } else {
                _OnDestroy(type);
            }
        }

        private IEnumerator _StartDestroy(DestroyType type, float sec) {
            yield return new WaitForSeconds(sec);
            _OnDestroy(type);
        }
        private void _OnDestroy(DestroyType type) {
            foreach (var item in _removeList) {
                _destroyListeners.Remove(item);
            }
            foreach (var item in _destroyListeners) {
                item.Value.Invoke(type);
            }
            if (_actionDestroy != null) {
                _actionDestroy.Invoke(type);
            } else {
                Destroy(gameObject);
            }
        }
    }
}