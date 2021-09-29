using UnityEngine;

namespace Game.Modules {
	public static class RacketFactory {
		public static UnitRacket Create(int id, Vector3 position, Transform parent) {
			var obj = RacketCache.instance.New(id);
			if (!obj) { return null; }
			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				RacketCache.instance.Delete(obj);
			});

			var ret = obj.GetComponent<UnitRacket>();
			ret.ResetDirection();
			ret.Init();

			return ret;
		}
	}
}