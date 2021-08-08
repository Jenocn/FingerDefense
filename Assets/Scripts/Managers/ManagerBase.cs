namespace Game.Managers {
	public interface IGameManager {
		void OnInitManager();
		void OnDestroyManager();
		void OnArchiveLoaded();
		void OnArchiveSaveBegin();
	}

	/// <summary>
	/// ManagerBase
	/// </summary>
	public class ManagerBase<T> : GCL.Base.ClassType<T, IGameManager>, IGameManager where T : ManagerBase<T>, new() {
        public static implicit operator bool(ManagerBase<T> exists) {
			return exists != null;
		}
		public virtual void OnInitManager() {}
		public virtual void OnDestroyManager() {}
		public virtual void OnArchiveLoaded() {}
		public virtual void OnArchiveSaveBegin() {}
	}
}