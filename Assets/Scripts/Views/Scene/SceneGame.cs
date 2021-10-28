using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {

    public class SceneGame : MonoBehaviour {

        private UiStack _uiStack = null;
        private MapManager _mapManager = null;

        // Start is called before the first frame update
        void Start() {
            _mapManager = ManagerCenter.GetManager<MapManager>();

            _uiStack = GetComponent<UiStack>();
            _uiStack.PushUI<UiGameNormal>();

            MessageCenter.AddListener<UiMessage_OnButtonGameAgain>(this, (UiMessage_OnButtonGameAgain msg) => {
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<UiMessage_OnButtonGameBack>(this, (UiMessage_OnButtonGameBack msg) => {
                var mapMode = _mapManager.mapMode;
                if (mapMode == MapMode.Classic) {
                    SceneManager.LoadScene("LevelSelectScene", LoadSceneMode.Single);
                } else {
                    SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
                }
            });
            MessageCenter.AddListener<UiMessage_OnButtonGameNextLevel>(this, (UiMessage_OnButtonGameNextLevel msg) => {
                _mapManager.SetCurrentNext();
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<MessageGameOver>(this, (MessageGameOver msg) => {
                if (msg.mapMode == MapMode.Classic) {
                    if (msg.bWined) {
                        var ui = _uiStack.PushUI<UiGameWined>();
                        ui.ShowData(msg.score, msg.highHitCount, msg.bHighestScore);
                    } else {
                        _uiStack.PushUI<UiGameFailed>();
                    }
                } else {
                    var ui = _uiStack.PushUI<UiGameOver>();
                    ui.ShowData(msg.score, msg.highHitCount, msg.bHighestScore);
                }

                GameApplication.SaveArchive();
            });
        }

        private void OnDestroy() {
            MessageCenter.RemoveListener<UiMessage_OnButtonGameAgain>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonGameBack>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonGameNextLevel>(this);
            MessageCenter.RemoveListener<MessageGameOver>(this);
        }
    }
}