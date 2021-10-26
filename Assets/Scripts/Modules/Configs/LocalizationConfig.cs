using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

public class LocalizationConfig : MonoBehaviour {

    [System.Serializable]
    public class Config {
        public Font srcFont;
        public Font destFont;
        [Range(0, 2)]
        public float fontSizePercent = 1;
    }

    [System.Serializable]
    public class Item {
        public string key;
        [Header("本地化")]
        public Config[] configs;
    }

    [SerializeField]
    private Item[] _localizationItems;

    public Item[] localizationItems => _localizationItems;

    public bool TryGetValue(string key, out Item ret) {
        foreach (var item in localizationItems) {
            if (item.key == key) {
                ret = item;
                return true;
            }
        }
        ret = null;
        return false;
    }

    public bool TryGetCurrent(out Item ret) {
        return TryGetValue(LocalizationSystem.GetCurrentInfo().key, out ret);
    }
}