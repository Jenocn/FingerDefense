using Game.Managers;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {
    public class SceneHome : MonoBehaviour {
        private UiStack _uiStack = null;
        void Start() {
            _uiStack = GetComponent<UiStack>();
            _uiStack.PushUI<UiStart>();

            MessageCenter.AddListener<UiMessage_OnButtonRankMode>(this, (UiMessage_OnButtonRankMode msg) => {
                GotoNormalGame(1);
            });
            MessageCenter.AddListener<UiMessage_OnButtonInfiniteMode>(this, (UiMessage_OnButtonInfiniteMode msg) => {
                GotoNormalGame(1);
            });
            MessageCenter.AddListener<UiMessage_OnButtonChallengeMode>(this, (UiMessage_OnButtonChallengeMode msg) => {
                GotoNormalGame(1);
            });
        }

        private void OnDestroy() {
            MessageCenter.RemoveListener<UiMessage_OnButtonRankMode>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonInfiniteMode>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonChallengeMode>(this);
        }

        void GotoNormalGame(int id) {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            var mapManager = ManagerCenter.GetManager<MapManager>();
            mapManager.SetCurrentID(id);
        }
    }
}