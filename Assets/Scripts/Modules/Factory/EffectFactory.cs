using UnityEngine;

namespace Game.Modules {
	public static class EffectFactory {
		public static UnitEffect Create(int id, Vector3 position, Transform parent) {
			var obj = EffectCache.instance.New(id);
			if (!obj) { return null; }

			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				Delete(obj);
			});

			var ret = obj.GetComponent<UnitEffect>();
			ret.Init();
			return ret;
		}
		public static void Delete(GameObject effectObject) {
			EffectCache.instance.Delete(effectObject);
		}
	}
}