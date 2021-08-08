using Game.Managers;
using Game.Systems;
using UnityEngine;

namespace Game.Modules {
	public class GameInput : MonoBehaviour {
		private delegate bool _IsCommand(InputCommand command);
		private delegate Vector2 _GetAxis(AxisID axisID);
		private delegate bool _IsAxis(AxisID axisID);
		private _IsCommand _IsDown = null;
		private _IsCommand _IsUp = null;
		private _IsCommand _IsHold = null;
		private _GetAxis _GetAxisCurrent = null;
		private _GetAxis _GetAxisPrev = null;
		private _GetAxis _GetAxisDelta = null;
		private _IsAxis _IsAxisActive = null;

		public bool IsDown(InputCommand command) {
			return _IsDown.Invoke(command);
		}
		public bool IsUp(InputCommand command) {
			return _IsUp.Invoke(command);
		}
		public bool IsHold(InputCommand command) {
			return _IsHold.Invoke(command);
		}

		public bool IsAxisActive(AxisID id) {
			return _IsAxisActive.Invoke(id);
		}

		public Vector2 GetAxisCurrent(AxisID id) {
			return _GetAxisCurrent.Invoke(id);
		}

		public Vector2 GetAxisPrev(AxisID id) {
			return _GetAxisPrev.Invoke(id);
		}

		public Vector2 GetAxisDelta(AxisID id) {
			return _GetAxisDelta.Invoke(id);
		}

		private void OnEnable() {
			_IsDown = (InputCommand command) => {
				return _GetButtonValue(command).down;
			};
			_IsUp = (InputCommand command) => {
				return _GetButtonValue(command).up;
			};
			_IsHold = (InputCommand command) => {
				return _GetButtonValue(command).hold;
			};
			_GetAxisCurrent = (AxisID id) => {
				return _GetAxisValue(id).cur;
			};
			_GetAxisPrev = (AxisID id) => {
				return _GetAxisValue(id).prev;
			};
			_GetAxisDelta = (AxisID id) => {
				return _GetAxisValue(id).delta;
			};
			_IsAxisActive = (AxisID id) => {
				return _GetAxisValue(id).active;
			};
		}
		private void OnDisable() {
			_IsDown = (InputCommand command) => {
				return false;
			};
			_IsUp = _IsDown;
			_IsHold = _IsDown;

			_GetAxisCurrent = (AxisID id) => {
				return Vector2.zero;
			};
			_GetAxisPrev = _GetAxisCurrent;
			_GetAxisDelta = _GetAxisCurrent;
			_IsAxisActive = (AxisID id) => {
				return false;
			};
		}
		private InputSystem.ButtonValue _GetButtonValue(InputCommand command) {
			return InputSystem.GetButtonValue((int) command);
		}

		private InputSystem.AxisValue _GetAxisValue(AxisID id) {
			return InputSystem.GetAxisValue((int) id);
		}
	}
}