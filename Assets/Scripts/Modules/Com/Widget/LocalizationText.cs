using Game.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules {
    [RequireComponent(typeof(Text))]
    public class LocalizationText : MonoBehaviour {
        [SerializeField, Header("StringUI")]
        private string _key = "";
        [SerializeField, Header("是否启用公共配置")]
        private bool _useConfig = true;
        private Text _text = null;

        private Font _saveFont = null;
        private int _saveSize = 0;
        private bool _bChange = false;

        private void Awake() {
            _text = GetComponent<Text>();
            _saveFont = _text.font;
            _saveSize = _text.fontSize;
        }

        private void Start() {
            LocalizationSystem.message.AddListener(this, () => {
                ResetText();
            });

            ResetText();
        }

        private void OnDestroy() {
            LocalizationSystem.message.RemoveListener(this);
        }

        public void ResetText() {
            if (_useConfig) {
                if (LocalizationSystem.localizationConfig.TryGetCurrent(out var item)) {
                    foreach (var config in item.configs) {
                        if ((config.srcFont != null) && (config.destFont != null)) {
                            if (_saveFont.name == config.srcFont.name) {
                                _text.font = config.destFont;
                            }
                            _text.fontSize = (int) (_text.fontSize * config.fontSizePercent);
                            _bChange = true;
                            break;
                        }
                    }
                } else {
                    if (_bChange) {
                        _bChange = false;
                        if (LocalizationSystem.GetCurrentInfo().key == LocalizationSystem.DEFAULT_INFO.key) {
                            _text.font = _saveFont;
                            _text.fontSize = _saveSize;
                        }
                    }
                }
            }

            var element = Strings.StringUi.Get(_key);
            if (!string.IsNullOrEmpty(element)) {
                _text.text = element;
            }
        }
    }
}