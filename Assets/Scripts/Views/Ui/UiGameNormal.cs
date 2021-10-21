using Game.Modules;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiGameNormal : UiModel {

        private Text _textScore = null;

        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiGameNormal");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            var top = ui.Find("Top");
            if (top) {
                var buttonPause = top.Find("ButtonPause")?.GetComponent<StateButton>();
                buttonPause?.SetForceAction((int index) => {
                    MessageCenter.Send(new MessageGamePause(index == 0));
                });

                _textScore = top.Find("TextScore")?.GetComponent<Text>();
                _textScore.text = "0";
            }

            MessageCenter.AddListener<MessageScoreChange>(this, (MessageScoreChange msg) => {
                _textScore.text = msg.score.ToString();
            });
        }
        public override void OnDestroyUI() {
            MessageCenter.RemoveListener<MessageScoreChange>(this);
        }
    }
}