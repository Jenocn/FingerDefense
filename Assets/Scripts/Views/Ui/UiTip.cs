using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUiModel;
using Game.Systems;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Views
{
	public class UiTip : UiModel
	{
		private Transform _panel = null;
		private Text _text = null;
		public override void OnInitUI()
		{
			var prefab = AssetSystem.Load<GameObject>("prefabs", "UiTip");
			var ui = InstantiateUI(prefab).transform;
			_panel = ui.Find("Panel");
			_text = _panel.Find("Text").GetComponent<Text>();
			_text.text = "";
		}

		public override float OnShowUI()
		{
			_panel.transform.localScale = new Vector3(1, 0, 1);
			_panel.transform.DOScaleY(1, 0.1f);
			return 0;
		}

		public override float OnHideUI()
		{
			float time = 0.1f;
			_panel.transform.DOScaleY(0, time);
			return time;
		}

		public void ShowText(string str, float sec = 1)
		{
			_text.text = str;
			DOTween.Sequence().AppendInterval(sec).OnComplete(() =>
			{
				PopThisUI();
			});
		}
	}
}
