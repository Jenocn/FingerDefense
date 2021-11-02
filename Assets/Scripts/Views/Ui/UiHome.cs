using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;
using Game.Managers;
using Game.Strings;

namespace Game.Views {
    public class UiHome : UiModel {
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiHome");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            var buttonInfiniteMode = ui.Find("ButtonInfiniteMode").GetComponent<Button>();
            var buttonChallengeMode = ui.Find("ButtonChallengeMode").GetComponent<Button>();
            var panelLockInfinite = buttonInfiniteMode.transform.Find("PanelLock");
            var panelLockChallenge = buttonChallengeMode.transform.Find("PanelLock");

            var mapManager = ManagerCenter.GetManager<MapManager>();

            panelLockInfinite.gameObject.SetActive(mapManager.IsInfiniteLocked());
            panelLockChallenge.gameObject.SetActive(mapManager.IsChallengeLocked());

            ui.Find("ButtonRankMode").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonRankMode());
            });
            buttonInfiniteMode.onClick.AddListener(() => {
                if (mapManager.IsInfiniteLocked()) {
                    ShowTip(StringUi.Get("UiHome_InfiniteTip", ""));
                } else {
                    MessageCenter.Send(new UiMessage_OnButtonInfiniteMode());
                }
            });
            buttonChallengeMode.onClick.AddListener(() => {
                if (mapManager.IsChallengeLocked()) {
                    ShowTip(StringUi.Get("UiHome_ChallengeTip", ""));
                } else {
                    MessageCenter.Send(new UiMessage_OnButtonChallengeMode());
                }
            });
            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                PopThisUI();
            });
            ui.Find("ButtonSetting").GetComponent<Button>().onClick.AddListener(() => {
                uiStack.PushUI<UiSetting>();
            });
        }

        public void ShowTip(string s) {
            var uiTip = uiStack.PushUI<UiTip>();
            uiTip.ShowText(s);
        }
    }
}