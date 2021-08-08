using System.Collections.Generic;

namespace Game.Managers {
	public class ManagerContainer {
		private Dictionary<int, IGameManager> _managers = new Dictionary<int, IGameManager>();
		private System.Action _updateFunc = () => {};

		public T GetManager<T>() where T : ManagerBase<T>, new() {
			if (_managers.TryGetValue(ManagerBase<T>.GetClassType(), out var ret)) {
				return ret as T;
			}
			return null;
		}

		public void Register<T>() where T : ManagerBase<T>, new() {
			_managers[ManagerBase<T>.GetClassType()] = new T();
		}
		public void Unregister<T>() where T : ManagerBase<T>, new() {
			_managers.Remove(ManagerBase<T>.GetClassType());
		}
		public void UnregisterAll() {
			_managers.Clear();
		}

		public void OnInitManagers() {
			foreach (var item in _managers) {
				item.Value.OnInitManager();
			}
		}

		public void OnDestroyManagers() {
			foreach (var item in _managers) {
				item.Value.OnDestroyManager();
			}
		}

		public void OnArchiveLoaded() {
			foreach (var item in _managers) {
				item.Value.OnArchiveLoaded();
			}
		}

		public void OnArchiveSaveBegin() {
			foreach (var item in _managers) {
				item.Value.OnArchiveSaveBegin();
			}
		}
	}
}