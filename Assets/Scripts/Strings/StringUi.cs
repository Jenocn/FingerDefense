using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Strings {
    public class StringUi : TableBase<StringUi, string, string> {
        public override void Load() {
            var text = AssetSystem.Load<TextAsset>("strings", "ui_" + LocalizationSystem.language)?.text;
            if (!string.IsNullOrEmpty(text)) {
                Assign(JSONTool.ParseToKV(text));
            }
        }
    }
}