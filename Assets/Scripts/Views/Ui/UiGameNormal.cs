using Game.Modules;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityUiModel;

namespace Game.Views {
    public class UiGameNormal : UiModel {
        private bool _bGamePause = false;
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiGameNormal");
            if (!prefab) {
                return;
            }
            var ui = Instantiate(prefab, transform).transform;
            var top = ui.Find("Top");
            if (top) {
                var buttonPause = top.Find("ButtonPause")?.GetComponent<StateButton>();
                buttonPause.SetForceAction((int index) => {
                    _bGamePause = (index == 0);
                    var msg = new MessageGamePause();
                    msg.bPause = _bGamePause;
                    MessageCenter.Send(msg);
                });
            }
        }
    }
}