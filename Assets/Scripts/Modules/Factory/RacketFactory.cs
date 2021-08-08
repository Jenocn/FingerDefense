using UnityEngine;

namespace Game.Modules {
	public static class RacketFactory {
		public static UnitRacket Create(int id, Vector3 position, Transform parent) {
			var obj = RacketCache.instance.New(id);
			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				RacketCache.instance.Delete(obj);
			});

			var ret = obj.GetComponent<UnitRacket>();
			ret.Init();

			return ret;
		}
	}
}