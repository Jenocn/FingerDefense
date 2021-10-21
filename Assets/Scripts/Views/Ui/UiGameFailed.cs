using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiGameFailed : UiModel {
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiGameFailed");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonGameBack());
            });
            ui.Find("ButtonAgain").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonGameAgain());
            });
        }
    }
}