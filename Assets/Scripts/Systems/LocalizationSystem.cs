using GCL.Pattern;

namespace Game.Systems {
	public static class LocalizationSystem {
		public static string language { get; private set; } = "chs";
		public static SimpleNotifyVoid message { get; private set; } = new SimpleNotifyVoid();

		public static void Init() {
			language = ArchiveSystem.common.GetString("LocalizationSystem", "Language", "chs");
		}

		public static void ChangeLanguage(string value) {
			language = value;
			ArchiveSystem.common.SetString("LocalizationSystem", "Language", language);
			message.Send();
		}
	}
}