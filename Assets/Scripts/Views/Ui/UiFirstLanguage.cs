using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
	public class UiFirstLanguage : UiModel {
		private bool _bOK = false;
		public override void OnInitUI() {
			var prefab = AssetSystem.Load<GameObject>("prefabs", "UiFirstLanguage");
			if (!prefab) {
				return;
			}
			var ui = InstantiateUI(prefab).transform;

			var root = ui.Find("Root");

			var buttonPrefab = AssetSystem.Load<GameObject>("prefabs", "UiFirstLanguage_Button");
			var infoList = LocalizationSystem.infoList;
			var count = infoList.Count;
			if (buttonPrefab && (count > 0)) {
				var padding = 5;
				for (var i = 0; i < count; ++i) {
					var info = infoList[i];
					var button = Instantiate<GameObject>(buttonPrefab, root).GetComponent<RectTransform>();
					button.Find("Text").GetComponent<Text>().text = info.show;

					float sizeY = button.sizeDelta.y + padding;
					button.localPosition = new Vector3(0, i * sizeY - sizeY * 0.5f * count, 0);

					var index = i;
					button.GetComponent<Button>().onClick.AddListener(() => {
						LocalizationSystem.ChangeLanguage(index);
						PopThisUI();
						GameApplication.SaveCommonArchive();
					});
				}
				_bOK = true;
			}
		}

		public override void OnStartUI() {
			if (!_bOK) {
				PopThisUI();
			}
		}
	}
}