using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUiModel;

namespace Game.Views {
    public class SceneHome : MonoBehaviour {
        private UiStack _uiStack = null;
        void Start() {
            _uiStack = GetComponent<UiStack>();
        }

        void Update() {
            if (Input.GetMouseButtonDown(1) ||
                Input.touchCount > 0) {
                GotoNormalGame(1);
            }
        }

        void GotoNormalGame(int id) {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
            var mapManager = ManagerCenter.GetManager<MapManager>();
            mapManager.SetCurrentID(id);
        }
    }
}