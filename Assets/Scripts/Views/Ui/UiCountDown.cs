using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiCountDown : UiModel {
        private Text _text = null;
        private float _countTime = 0;
        private System.Action _action = null;
        private bool _bPlay = false;
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiCountDown");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            _text = ui.Find("Text").GetComponent<Text>();
        }

        private void Update() {
            if (_bPlay) {
                if (_countTime > 0) {
                    _countTime -= Time.deltaTime;

                    _RefreshText();

                    if (_countTime <= 0) {
                        if (_action != null) {
                            _action();
                        }
                        PopThisUI();
                    }
                }
            }
        }

        public void Play(int time, System.Action action) {
            _bPlay = true;
            _countTime = time;
            _action = action;
            _RefreshText();
        }

        private void _RefreshText() {
            _text.text = Mathf.Max(0, Mathf.CeilToInt(_countTime)).ToString();
        }

    }
}