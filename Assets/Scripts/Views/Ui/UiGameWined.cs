using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiGameWined : UiModel {
        private Text _textScoreHigh = null;
        private Text _textScore = null;
        private Text _textComboHit = null;
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiGameWined");
            if (!prefab) {
                return;
            }
            var ui = InstantiateUI(prefab).transform;
            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonGameBack());
            });
            var buttonNext = ui.Find("ButtonNext");
            buttonNext.GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonGameNextLevel());
            });
            buttonNext.gameObject.SetActive(ManagerCenter.GetManager<MapManager>().HasClassicNext());

            _textScoreHigh = ui.Find("TextScoreHigh").GetComponent<Text>();
            _textScore = ui.Find("TextScore").GetComponent<Text>();
            _textComboHit = ui.Find("TextComboHit").GetComponent<Text>();

            _textScoreHigh.gameObject.SetActive(false);
            _textScore.gameObject.SetActive(false);
            _textComboHit.gameObject.SetActive(false);
        }

        public void ShowData(int score, int hit, bool bHigh) {
            _textScoreHigh.text = score.ToString();
            _textScore.text = score.ToString();
            _textComboHit.text = hit.ToString();
            _textScoreHigh.gameObject.SetActive(bHigh);
            _textScore.gameObject.SetActive(!bHigh);
            _textComboHit.gameObject.SetActive(true);
        }
    }
}