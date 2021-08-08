using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUiModel;

namespace Game.Views {
    public class SceneMapEditor : MonoBehaviour {
        private UiStack _uiStack = null;

        // Start is called before the first frame update
        void Start() {
            _uiStack = GetComponent<UiStack>();
            _uiStack.PushUI<UiEditorMap>();
        }

    }
}