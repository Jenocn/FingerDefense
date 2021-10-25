using System;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Modules {
	public sealed class StateMachine : MonoBehaviour {

		/// <summary>
		/// 当前日志器
		/// </summary>
		public LogSystem.Logger logger { get; private set; }

		/// <summary>
		/// 当前State
		/// </summary>
		public StateBase currentState { get; private set; } = StateBase.EMPTY;

		// 历史State
		private LinkedList<StateBase> _history = new LinkedList<StateBase>();
		private int _historyLength = 20;

		// ChangeState调用保护
		private bool _bChangeStateSafe = false;
		private bool _bNewState = false;

#if UNITY_EDITOR
		// display
		[SerializeField]
		[DisplayOnly]
		private string _display = "";
#endif

		private void Awake() {
			logger = LogSystem.GetLogger("UnitState");
		}

		private void Update() {
			if (_bNewState) {
				currentState.OnStateStart();
				_bNewState = false;
			}
			currentState.OnStateUpdate();
		}

		private void OnDestroy() {
			_ChangeState(StateBase.EMPTY, false);
		}

		public void ChangeState<T>() where T : StateBase, new() {
			_ChangeState(new T(), false);
		}

		public void ChangeState(Type type) {
			var state = Activator.CreateInstance(type) as StateBase;
			_ChangeState(state, false);
		}

		public bool IsRunningState<T>() where T : StateBase {
			return typeof(T).IsInstanceOfType(currentState);
		}

		public void RevertState() {
			if (_history.Count == 0) { return; }
			var state = _history.Last.Value;
			_history.RemoveLast();
			var newState = Activator.CreateInstance(state.GetType()) as StateBase;
			_ChangeState(state, true);
		}

		private void _ChangeState(StateBase state, bool bRevert) {
			if (_bChangeStateSafe) {
				logger.LogError(state.GetType().Name + ": Can't 'ChangeState' in 'OnStateCreate() or OnStateDestroy()'!");
				return;
			}
			_bChangeStateSafe = true;
			currentState.OnStateDestroy();
			if (!bRevert) {
				_AddHistory(currentState);
			}
			currentState = state;
			currentState._Init(gameObject, this);
			currentState.OnStateCreate();
			_bChangeStateSafe = false;
			_bNewState = true;

#if UNITY_EDITOR
			_display = state.GetType().Name;
#endif
		}

		private void _AddHistory(StateBase state) {
			_history.AddLast(state);
			if (_history.Count > _historyLength) {
				_history.RemoveFirst();
			}
		}
	}
}