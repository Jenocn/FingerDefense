using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {

    public class SceneGame : MonoBehaviour {

        private UiStack _uiStack = null;

        // Start is called before the first frame update
        void Start() {
            _uiStack = GetComponent<UiStack>();
            _uiStack.PushUI<UiGameNormal>();

            MessageCenter.AddListener<UiMessage_OnButtonGameAgain>(this, (UiMessage_OnButtonGameAgain msg) => {
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<UiMessage_OnButtonGameBack>(this, (UiMessage_OnButtonGameBack msg) => {
                SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
            });
            MessageCenter.AddListener<MessageGameFailed>(this, (MessageGameFailed msg) => {
                _uiStack.PushUI<UiGameFailed>();
            });
        }

        private void OnDestroy() {
            MessageCenter.RemoveListener<UiMessage_OnButtonGameAgain>(this);
            MessageCenter.RemoveListener<UiMessage_OnButtonGameBack>(this);
            MessageCenter.RemoveListener<MessageGameFailed>(this);
        }
    }
}