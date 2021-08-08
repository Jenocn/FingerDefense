using UnityEngine;

namespace Game.Modules {
	public static class BrickFactory {
		public static UnitBrick Create(int id, Vector3 position, Transform parent) {
			var obj = BrickCache.instance.New(id);
			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				BrickCache.instance.Delete(obj);
			});

			var ret = obj.GetComponent<UnitBrick>();

			return ret;
		}
	}
}