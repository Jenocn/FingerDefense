namespace Game.Managers {
	public interface IGameManager {
		void OnInitManager();
		void OnStartManager();
		void OnDestroyManager();
		void OnArchiveLoaded();
		void OnArchiveSaveBegin();
		void OnSceneLoaded();
		void OnSceneUnloaded();
	}

	/// <summary>
	/// ManagerBase
	/// </summary>
	public class ManagerBase<T> : GCL.Base.ClassType<T, IGameManager>, IGameManager where T : ManagerBase<T>, new() {
		public static implicit operator bool(ManagerBase<T> exists) {
			return exists != null;
		}
		public virtual void OnInitManager() {}
		public virtual void OnStartManager() {}
		public virtual void OnDestroyManager() {}
		public virtual void OnArchiveLoaded() {}
		public virtual void OnArchiveSaveBegin() {}
		public virtual void OnSceneLoaded() {}
		public virtual void OnSceneUnloaded() {}
	}
}