namespace Game.Managers {
	/// <summary>
	/// Managers
	/// </summary>
	public static class ManagerCenter {
		private static ManagerContainer _managers = new ManagerContainer();
		private static void _RegisterManagers() {
			_managers.Register<ScriptManager>();
			_managers.Register<StringManager>();
			_managers.Register<TableManager>();
			_managers.Register<AudioManager>();
			_managers.Register<InputManager>();
			_managers.Register<DamageManager>();
			_managers.Register<MapManager>();
			_managers.Register<ScoreManager>();
		}

		public static T GetManager<T>() where T : ManagerBase<T>, new() {
			return _managers.GetManager<T>();
		}

		public static void OnInitManagers() {
			_RegisterManagers();
			_managers.OnInitManagers();
		}

		public static void OnStartManagers() {
			_managers.OnStartManagers();
		}
		
		public static void OnDestroyManagers() {
			_managers.OnDestroyManagers();
			_managers.UnregisterAll();

		}
		public static void OnArchiveLoaded() {
			_managers.OnArchiveLoaded();
		}
		public static void OnArchiveSaveBegin() {
			_managers.OnArchiveSaveBegin();
		}

		public static void OnSceneLoaded() {
			_managers.OnSceneLoaded();
		}
		public static void OnSceneUnloaded() {
			_managers.OnSceneUnloaded();
		}
	}
}