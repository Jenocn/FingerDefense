using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Strings {
    public class StringTalk : TableBase<StringTalk, string, string> {
        public override void Load() {
            var text = AssetSystem.Load<TextAsset>("strings", "talk")?.text;
            if (!string.IsNullOrEmpty(text)) {
                Assign(JSONTool.ParseToKV(text));
            }
        }
    }
}