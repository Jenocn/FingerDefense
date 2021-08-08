using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(Camera))]
    public class CameraViewportAuto : MonoBehaviour {

        [SerializeField]
        private Vector2 _resolution = Vector2.zero;
        [SerializeField]
        private bool _stretchFill = true;

        [SerializeField]
        private int _piexlPerUnit = 100;
        private void Awake() {
            UpdateViewportRect();
        }

        public void UpdateViewportRect() {
            if (_resolution.x <= 0 || _resolution.y <= 0) {
                return;
            }

            float perX = 1;
            float perY = 1;
            if (_stretchFill) {
                perX = Screen.width / _resolution.x;
                perY = Screen.height / _resolution.y;
            } else {
                perX = _resolution.x / Screen.width;
                perY = _resolution.y / Screen.height;
            }

            var fper = Mathf.Min(perX, perY);
            var fw = _resolution.x * fper;
            var fh = _resolution.y * fper;

            var x = (Screen.width - fw) * 0.5f / Screen.width;
            var y = (Screen.height - fh) * 0.5f / Screen.height;
            var w = fw / Screen.width;
            var h = fh / Screen.height;

            var camera = GetComponent<Camera>();
            var orthographicSize = _resolution.y / _piexlPerUnit * 0.5f;
            camera.orthographicSize = orthographicSize;
            camera.rect = new Rect(x, y, w, h);
        }
    }
}