using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiHome : UiModel {
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiHome");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            ui.Find("ButtonRankMode").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonRankMode());
            });
            ui.Find("ButtonInfiniteMode").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonInfiniteMode());
            });
            ui.Find("ButtonChallengeMode").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonChallengeMode());
            });
            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                PopThisUI();
            });
            ui.Find("ButtonSetting").GetComponent<Button>().onClick.AddListener(() => {
                uiStack.PushUI<UiSetting>();
            });
        }
    }
}