using System.Collections;
using System.Collections.Generic;
using Game.Modules;
using Game.Systems;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

public class UiSetting : UiModel {
    public override void OnInitUI() {
        var prefab = AssetSystem.Load<GameObject>("prefabs", "UiSetting");
        if (!prefab) {
            return;
        }
        var ui = InstantiateUI(prefab).transform;
        var textTitle = ui.Find("TextTitle").GetComponent<LocalizationText>();

        var optionMusic = ui.Find("OptionMusic").transform;
        var textMusic = optionMusic.Find("TextSign").GetComponent<LocalizationText>();

        var optionSE = ui.Find("OptionSoundEffect").transform;
        var textSE = optionSE.Find("TextSign").GetComponent<LocalizationText>();

        var optionLanguage = ui.Find("OptionLanguage").transform;
        var textLanguageSign = optionLanguage.Find("TextSign").GetComponent<LocalizationText>();
        var textLanguageValue = optionLanguage.Find("TextValue").GetComponent<LocalizationText>();

        var optionBack = ui.Find("ButtonBack").transform;
        var textBack = optionBack.Find("TextSign").GetComponent<LocalizationText>();

        optionBack.GetComponent<Button>().onClick.AddListener(() => {
            uiStack.PopUI();
        });

        optionLanguage.Find("ButtonLeft").GetComponent<Button>().onClick.AddListener(() => {
            LocalizationSystem.ChangeLanguageBefore();
        });
        optionLanguage.Find("ButtonRight").GetComponent<Button>().onClick.AddListener(() => {
            LocalizationSystem.ChangeLanguageNext();
        });

        LocalizationSystem.message.AddListener(this, () => {
            textTitle.ResetText();
            textMusic.ResetText();
            textSE.ResetText();
            textLanguageSign.ResetText();
            textLanguageValue.ResetText();
            textBack.ResetText();
        });
    }
    public override void OnDestroyUI() {
        LocalizationSystem.message.RemoveListener(this);

    }
}