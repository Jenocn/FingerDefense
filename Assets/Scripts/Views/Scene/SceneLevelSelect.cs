using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {

    public class SceneLevelSelect : MonoBehaviour {

        private UiStack _uiStack = null;
        private ScriptManager _scriptManager = null;

        private void Awake() {
            _uiStack = GetComponent<UiStack>();
            _scriptManager = ManagerCenter.GetManager<ScriptManager>();
        }

        private void Start() {
            _uiStack.PushUI<UiLevelSelect>();

            MessageCenter.AddListener<UiMessage_OnClassicLevelSelect>(this, (UiMessage_OnClassicLevelSelect msg) => {
                var mapManager = ManagerCenter.GetManager<MapManager>();
                mapManager.SetCurrent(MapMode.Classic, msg.mapID);
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<UiMessage_OnButtonGameBack>(this, (UiMessage_OnButtonGameBack msg) => {
                SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
            });

            _scriptManager.ExecuteWithCache("trigger.peak", "level_select_loaded");
        }

        private void OnDestroy() {
            MessageCenter.RemoveListener<UiMessage_OnClassicLevelSelect>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonGameBack>(this);
        }
    }
}