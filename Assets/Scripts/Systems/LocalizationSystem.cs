using System.Collections.Generic;
using GCL.Pattern;
using GCL.Serialization;
using UnityEngine;

namespace Game.Systems {
	public static class LocalizationSystem {
		public class Info {
			public string key = "";
			public string show = "";
			public string fileSign = "";
		}

		public static SimpleNotifyVoid message { get; private set; } = new SimpleNotifyVoid();
		public static Info DEFAULT_INFO { get; private set; }
		public static List<Info> infoList => _infoList;
		public static LocalizationConfig localizationConfig { get; private set; } = null;

		private static List<Info> _infoList = new List<Info>();
		private static int _index = 0;
		private static Transform transform = null;

		public static void Init() {
			var src = AssetSystem.Load<TextAsset>("drive", "localization.ini").text;
			var datas = INITool.Parse(src);
			foreach (var item in datas) {
				var info = new Info();
				info.key = item.Key;
				info.show = item.Value["Show"];
				info.fileSign = item.Value["FileSign"];
				_infoList.Add(info);
			}

			DEFAULT_INFO = _infoList[0];

			var key = ArchiveSystem.common.GetString("LocalizationSystem", "Language", DEFAULT_INFO.key);

			var index = _infoList.FindIndex((Info value) => {
				return value.key == key;
			});

			if (index >= 0 && index < _infoList.Count) {
				_index = index;
			} else {
				_index = 0;
			}

			var prefab = AssetSystem.Load<GameObject>("prefabs", "LocalizationObject");
			if (prefab) {
				var obj = Object.Instantiate<GameObject>(prefab);
				Object.DontDestroyOnLoad(obj);
				transform = obj.transform;
				localizationConfig = transform.GetComponent<LocalizationConfig>();
			}
		}

		/// <summary>
		/// 一定返回有效值
		/// </summary>
		public static Info GetCurrentInfo() {
			if (_index >= 0 && _index < _infoList.Count) {
				return _infoList[_index];
			}
			return DEFAULT_INFO;
		}

		public static void ChangeLanguage(int index) {
			if (index < 0) {
				index = 0;
			}
			if (index >= _infoList.Count) {
				index = _infoList.Count - 1;
			}

			if (_index != index) {
				_index = index;
				message.Send();
				ArchiveSystem.common.SetString("LocalizationSystem", "Language", _infoList[_index].key);
			}
		}

		public static void ChangeLanguage(string key) {
			var index = _infoList.FindIndex((Info info) => {
				return info.key == key;
			});
			ChangeLanguage(index);
		}

		public static void ChangeLanguageBefore() {
			var newIndex = _index - 1;
			if (newIndex < 0) {
				newIndex = _infoList.Count - 1;
			}
			ChangeLanguage(newIndex);
		}

		public static void ChangeLanguageNext() {
			var newIndex = _index + 1;
			if (newIndex >= _infoList.Count) {
				newIndex = 0;
			}
			ChangeLanguage(newIndex);
		}
	}
}