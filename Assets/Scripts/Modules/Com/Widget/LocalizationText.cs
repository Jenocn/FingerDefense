using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules {
    [RequireComponent(typeof(Text))]
    public class LocalizationText : MonoBehaviour {
        [SerializeField]
        private string _key = "";
        private Text _text = null;
        private void Awake() {
            _text = GetComponent<Text>();
        }

        private void Start() {
            ResetText();
        }

        public void ResetText() {
            var element = Strings.StringUi.instance.GetElement(_key);
            if (!string.IsNullOrEmpty(element)) {
                _text.text = element;
            }
        }
    }
}