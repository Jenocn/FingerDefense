using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiStart : UiModel {
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiStart");
            if (!prefab) {
                return;
            }
            InstantiateUI(prefab);
        }

        private void Update() {
            if (Input.anyKeyDown) {
                uiStack.PushUI<UiHome>();
            }
        }

        public override void OnTopBackUI() {
            enabled = true;
        }
        public override void OnTopLostUI() {
            enabled = false;
        }
    }
}