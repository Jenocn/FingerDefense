using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Strings {
    public class StringUi : TableBase<StringUi, string, string> {
        public static string Get(string key, string def = "") {
            var element = instance.GetElement(key);
            if (element != null) {
                return element;
            }
            return def;
        }
        public override void Load() {
            var currentInfo = LocalizationSystem.GetCurrentInfo();
            var text = AssetSystem.Load<TextAsset>("strings", "ui_" + currentInfo.fileSign)?.text;
            if (string.IsNullOrEmpty(text)) {
                text = AssetSystem.Load<TextAsset>("strings", "ui")?.text;
            }
            if (!string.IsNullOrEmpty(text)) {
                Assign(JSONTool.ParseToKV(text));
            }
        }
    }
}