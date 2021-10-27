using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Strings {
    public class StringTalk : TableBase<StringTalk, string, string> {
        public static string Get(string key, string def = "") {
            var element = instance.GetElement(key);
            if (element != null) {
                return element;
            }
            return def;
        }
        public override void Load() {
            var currentInfo = LocalizationSystem.GetCurrentInfo();
            var text = AssetSystem.Load<TextAsset>("strings", "talk_" + currentInfo.fileSign)?.text;
            if (string.IsNullOrEmpty(text)) {
                text = AssetSystem.Load<TextAsset>("strings", "talk")?.text;
            }
            if (!string.IsNullOrEmpty(text)) {
                Assign(JSONTool.ParseToKV(text));
            }
        }
    }
}