using UnityEngine;

namespace Game.Modules {
	public static class EffectFactory {
		public static UnitBomb CreateBomb(int id, Vector3 position, Transform parent) {
			var obj = EffectCache.instance.New(id);
			if (!obj) { return null; }

			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				EffectCache.instance.Delete(obj);
			});

			var ret = obj.GetComponent<UnitBomb>();
			ret.Init();
			return ret;
		}
	}
}