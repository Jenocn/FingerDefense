using UnityEngine;

namespace Game.Modules {
	public static class BallFactory {
		public static UnitBall Create(int id, Vector3 position, Transform parent, Vector2 direction) {
			var obj = BallCache.instance.New(id);
			if (!obj) { return null; }
			obj.transform.SetParent(parent);
			obj.transform.position = position;

			var unitDestroy = obj.GetComponent<UnitDestroy>();
			unitDestroy.ResetDestroyFunc((DestroyType type) => {
				Delete(obj);
			});

			var ret = obj.GetComponent<UnitBall>();
			ret.SetDirection(direction);

			return ret;
		}
		public static void Delete(GameObject ballObject) {
			BallCache.instance.Delete(ballObject);
		}
	}
}