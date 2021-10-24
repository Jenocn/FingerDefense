using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    public class EffectCache : Singleton<EffectCache> {
        private Dictionary<int, CachePool> _cachePool = new Dictionary<int, CachePool>();
        public GameObject New(int uniqueID) {
            if (_cachePool.TryGetValue(uniqueID, out var pool)) {
                if (!pool.Empty()) {
                    return pool.Pop();
                }
            }
            var prefab = AssetSystem.Load<GameObject>("prefabs", "effect_" + uniqueID);
            if (prefab) {
                return Object.Instantiate(prefab);
            }
            return null;
        }
        public bool Delete(GameObject gameObject) {
            var unitID = gameObject.GetComponent<UnitID>();
            if (!unitID) {
                Object.Destroy(gameObject);
                return false;
            }
            if (!_cachePool.TryGetValue(unitID.uniqueID, out var pool)) {
                pool = new CachePool();
                _cachePool.Add(unitID.uniqueID, pool);
            }
            pool.Push(gameObject);
            return true;
        }
        public void Clear() {
            foreach (var item in _cachePool) {
                item.Value.Clear(true);
            }
            _cachePool.Clear();
        }
    }
}