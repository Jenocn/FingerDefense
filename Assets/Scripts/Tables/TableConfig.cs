using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Tables {
	public class TableConfigElement : DataObject {
		public TableConfigElement(string value) {
			this.value = value;
		}
		public string value { get; private set; }

		public int ToInt(int def = 0) {
			if (int.TryParse(value, out var ret)) {
				return ret;
			}
			return def;
		}
		public float ToFloat(float def = 0) {
			if (float.TryParse(value, out var ret)) {
				return ret;
			}
			return def;
		}
		public override string ToString() {
			return value;
		}

		public bool ToBool(bool def = false) {
			if (bool.TryParse(value.ToLower(), out var ret)) {
				return ret;
			}
			return def;
		}
	}

	public class TableConfig : TableBase<TableConfig, string, TableConfigElement> {
		public static string GetString(string key, string def = "") {
			var element = instance.GetElement(key);
			if (element) {
				return element.value;
			}
			return def;
		}
		public static int GetInt(string key, int def = 0) {
			var element = instance.GetElement(key);
			if (element) {
				return element.ToInt(def);
			}
			return def;
		}
		public static float GetFloat(string key, float def = 0) {
			var element = instance.GetElement(key);
			if (element) {
				return element.ToFloat(def);
			}
			return def;
		}
		public static bool GetBool(string key, bool def = false) {
			var element = instance.GetElement(key);
			if (element) {
				return element.ToBool(def);
			}
			return def;
		}

		public override void Load() {
			var src = AssetSystem.Load<TextAsset>("tables", "config")?.text;
			if (string.IsNullOrEmpty(src)) {
				return;
			}
			var rets = JSONTool.ParseToKV(src);
			foreach (var item in rets) {
				Emplace(item.Key, new TableConfigElement(item.Value));
			}
		}
	}
}