using Game.Managers;
using Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiScoreShow : UiModel {
        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiScoreShow");
            var ui = InstantiateUI(prefab).transform;
            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                PopThisUI();
            });

            var text = ui.Find("TextScore").GetComponent<Text>();
            var scoreManager = ManagerCenter.GetManager<ScoreManager>();
            text.text += "\n";
            foreach (var item in scoreManager.classicHighestScoreDict) {
                text.text += ("ID: " + item.Key + ", Score: " + item.Value + "\n");
            }
        }
    }
}