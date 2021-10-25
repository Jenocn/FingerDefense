using Game.Managers;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {
    public class SceneHome : MonoBehaviour {
        private UiStack _uiStack = null;
        private ScriptManager _scriptManager = null;
        void Start() {
            _scriptManager = ManagerCenter.GetManager<ScriptManager>();

            _uiStack = GetComponent<UiStack>();
            _uiStack.PushUI<UiStart>();

            MessageCenter.AddListener<UiMessage_OnButtonRankMode>(this, (UiMessage_OnButtonRankMode msg) => {
                GotoNormalGame(MapMode.Classic, 1);
            });
            MessageCenter.AddListener<UiMessage_OnButtonInfiniteMode>(this, (UiMessage_OnButtonInfiniteMode msg) => {
                GotoNormalGame(MapMode.Infinite, 1);
            });
            MessageCenter.AddListener<UiMessage_OnButtonChallengeMode>(this, (UiMessage_OnButtonChallengeMode msg) => {
                GotoNormalGame(MapMode.Challenge, 1);
            });

            _scriptManager.ExecuteWithCache("trigger", "home_loaded");
        }

        private void OnDestroy() {
            MessageCenter.RemoveListener<UiMessage_OnButtonRankMode>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonInfiniteMode>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonChallengeMode>(this);
        }

        void GotoNormalGame(MapMode mode, int id) {
            var mapManager = ManagerCenter.GetManager<MapManager>();
            mapManager.SetCurrent(mode, id);
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}