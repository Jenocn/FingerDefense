using Game.Systems;
using GCL.Serialization;
using UnityEngine;

namespace Game.Strings {
    public class StringStory : TableBase<StringStory, string, string> {
        public override void Load() {
            var text = AssetSystem.Load<TextAsset>("strings", "story")?.text;
            if (!string.IsNullOrEmpty(text)) {
                Assign(JSONTool.ParseToKV(text));
            }
        }
    }
}