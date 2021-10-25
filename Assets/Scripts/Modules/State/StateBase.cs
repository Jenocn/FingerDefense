using Game.Systems;
using UnityEngine;

namespace Game.Modules {
	public class StateBase {
		public static StateBase EMPTY { get; } = new StateBase();
		public GameObject gameObject { get; private set; }
		public Transform transform { get => gameObject.transform; }
		protected StateMachine stateMachine { get; private set; }
		protected LogSystem.Logger logger { get => stateMachine.logger; }

		public void _Init(GameObject obj, StateMachine stateMachine) {
			gameObject = obj;
			this.stateMachine = stateMachine;
		}
		public virtual void OnStateCreate() { }
		public virtual void OnStateStart() { }
		public virtual void OnStateDestroy() { }
		public virtual void OnStateUpdate() { }

		public T GetComponent<T>() {
			return gameObject.GetComponent<T>();
		}
		public T GetComponentInChildren<T>() {
			return gameObject.GetComponentInChildren<T>();
		}

		public void ChangeState<T>() where T : StateBase, new() {
			stateMachine.ChangeState<T>();
		}
	}
}