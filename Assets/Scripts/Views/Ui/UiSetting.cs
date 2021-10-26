using Game.Managers;
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

        var optionMusic = ui.Find("OptionMusic").transform;
        var optionSE = ui.Find("OptionSoundEffect").transform;
        var optionLanguage = ui.Find("OptionLanguage").transform;

        var sliderMusic = optionMusic.Find("Slider").GetComponent<Slider>();
        var sliderSE = optionSE.Find("Slider").GetComponent<Slider>();

        var audioManager = ManagerCenter.GetManager<AudioManager>();
        
        sliderMusic.value = audioManager.GetVolume(MixerType.Music);
        sliderSE.value = audioManager.GetVolume(MixerType.Effect);

        sliderMusic.onValueChanged.AddListener((float v) => {
            audioManager.SetVolume(MixerType.Music, v);
        });
        sliderSE.onValueChanged.AddListener((float v) => {
            audioManager.SetVolume(MixerType.Effect, v);
        });

        ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
            GameApplication.SaveCommonArchive();
            uiStack.PopUI();
        });

        optionLanguage.Find("ButtonLeft").GetComponent<Button>().onClick.AddListener(() => {
            LocalizationSystem.ChangeLanguageBefore();
        });
        optionLanguage.Find("ButtonRight").GetComponent<Button>().onClick.AddListener(() => {
            LocalizationSystem.ChangeLanguageNext();
        });
    }
}