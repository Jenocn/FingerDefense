using System.Collections.Generic;
using GCL.Pattern;

namespace Game.Systems {
	public static class LocalizationSystem {
		public static string language { get; private set; } = "chs";
		public static SimpleNotifyVoid message { get; private set; } = new SimpleNotifyVoid();

		private static List<string> _languageList = new List<string>();
		private static int _index = 0;

		public static void Init() {
			_languageList.Add("chs");
			_languageList.Add("eng");

			language = ArchiveSystem.common.GetString("LocalizationSystem", "Language", "chs");
			var index = _languageList.FindIndex((string value) => {
				return value == language;
			});
			if (index >= 0 && index < _languageList.Count) {
				_index = index;
			} else {
				_index = 0;
				language = _languageList[0];
			}
		}

		public static void ChangeLanguage(int index) {
			_index = index;
			if (_index < 0) {
				_index = _languageList.Count - 1;
			}
			if (_index >= _languageList.Count) {
				_index = 0;
			}

			language = _languageList[_index];
			ArchiveSystem.common.SetString("LocalizationSystem", "Language", language);
			message.Send();
		}

		public static void ChangeLanguageBefore() {
			ChangeLanguage(_index - 1);
		}

		public static void ChangeLanguageNext() {
			ChangeLanguage(_index + 1);

		}
	}
}