using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Utility {
	public static class UnityUtility {
		public static T ForceGetComponent<T>(GameObject target) where T : Component {
			var ret = target.GetComponent<T>();
			if (!ret) {
				return target.AddComponent<T>();
			}
			return ret;
		}
		public static T ForceGetComponent<T>(Component target) where T : Component {
			return ForceGetComponent<T>(target.gameObject);
		}

		private static Material _defaultMaterial = null;
		public static Material GetDefaultMaterial() {
			if (!_defaultMaterial) {
				Shader shader = Shader.Find("Hidden/Internal-Colored");
				_defaultMaterial = new Material(shader);
				_defaultMaterial.hideFlags = HideFlags.HideAndDontSave;
				_defaultMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
				_defaultMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				_defaultMaterial.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
				_defaultMaterial.SetInt("_ZWrite", 0);
			}
			return _defaultMaterial;
		}

		public static void DrawLine(Vector3 pos0, Vector3 pos1, Color color, Material material = null) {
			var useMaterial = (material ? material : GetDefaultMaterial());
			useMaterial.SetPass(0);

			GL.PushMatrix();
			GL.Begin(GL.LINES);
			GL.Color(color);
			GL.Vertex(pos0);
			GL.Vertex(pos1);
			GL.End();
			GL.PopMatrix();
		}

		public static void ChangeVisible(Transform target, bool bShow, bool bChildren) {
			var renderer = target.GetComponents<Renderer>();
			foreach (var item in renderer) {
				item.enabled = bShow;
			}

			var uiBehaviour = target.GetComponents<UIBehaviour>();
			foreach (var item in uiBehaviour) {
				item.enabled = bShow;
			}

			if (bChildren) {
				for (int i = 0; i < target.childCount; ++i) {
					var child = target.GetChild(i);
					ChangeVisible(child, bShow, bChildren);
				}
			}
		}
	}
}