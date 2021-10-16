using System.Collections.Generic;

namespace Game.Managers {
	public class ManagerContainer {
		private Dictionary<int, IGameManager> _managers = new Dictionary<int, IGameManager>();
		private LinkedList<IGameManager> _managerSortList = new LinkedList<IGameManager>();

		public T GetManager<T>() where T : ManagerBase<T>, new() {
			if (_managers.TryGetValue(ManagerBase<T>.GetClassType(), out var ret)) {
				return ret as T;
			}
			return null;
		}

		public void Register<T>() where T : ManagerBase<T>, new() {
			var manager = new T();
			_managers[ManagerBase<T>.GetClassType()] = manager;
			_managerSortList.AddLast(manager);

		}
		public void Unregister<T>() where T : ManagerBase<T>, new() {
			var manager = GetManager<T>();
			if (manager) {
				_managerSortList.Remove(GetManager<T>());
				_managers.Remove(ManagerBase<T>.GetClassType());
			}
		}
		public void UnregisterAll() {
			_managers.Clear();
			_managerSortList.Clear();
		}

		public void OnInitManagers() {
			foreach (var item in _managerSortList) {
				item.OnInitManager();
			}
		}

		public void OnDestroyManagers() {
			var p = _managerSortList.Last;
			while (p != null) {
				p.Value.OnDestroyManager();
				p = p.Previous;
			}
		}

		public void OnArchiveLoaded() {
			foreach (var item in _managerSortList) {
				item.OnArchiveLoaded();
			}
		}

		public void OnArchiveSaveBegin() {
			foreach (var item in _managerSortList) {
				item.OnArchiveSaveBegin();
			}
		}
	}
}