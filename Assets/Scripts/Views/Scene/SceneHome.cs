using Game.Managers;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {
    public class SceneHome : MonoBehaviour {
        private const uint ArchiveIndex = 1;
        private UiStack _uiStack = null;
        private ScriptManager _scriptManager = null;
        private static bool _bTag = false;
        void Start() {
            _scriptManager = ManagerCenter.GetManager<ScriptManager>();

            _uiStack = GetComponent<UiStack>();

            if (_bTag) {
                _uiStack.PushUI<UiStart>();
                _uiStack.PushUI<UiHome>();
            } else {
                _uiStack.PushUI<UiStart>();
                _bTag = true;

                if (GameApplication.isFirstOpen) {
                    _uiStack.PushUI<UiFirstLanguage>();
                }

                GameApplication.LoadArchive(ArchiveIndex);
            }

            MessageCenter.AddListener<UiMessage_OnButtonRankMode>(this, (UiMessage_OnButtonRankMode msg) => {
                SceneManager.LoadScene("LevelSelectScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<UiMessage_OnButtonInfiniteMode>(this, (UiMessage_OnButtonInfiniteMode msg) => {
                GotoNormalGame(MapMode.Infinite, 1);
            });
            MessageCenter.AddListener<UiMessage_OnButtonChallengeMode>(this, (UiMessage_OnButtonChallengeMode msg) => {
                GotoNormalGame(MapMode.Challenge, 1);
            });

            _scriptManager.ExecuteWithCache("trigger.peak", "home_loaded");
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