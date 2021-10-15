using UnityEngine;

namespace Game.Modules {
	public static class BrickFactory {
		public static UnitBrick Create(int id, int hpMax, Vector3 position, Transform parent) {
			var obj = BrickCache.instance.New(id);
			if (!obj) { return null; }
			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				Delete(obj);
			});

			var ret = obj.GetComponent<UnitBrick>();
			ret.unitHealth.healthData.hpMax = hpMax;
			ret.Init();

			return ret;
		}
		public static void Delete(GameObject brickObject) {
			BrickCache.instance.Delete(brickObject);
		}
	}
}