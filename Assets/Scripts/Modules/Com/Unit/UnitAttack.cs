using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitID))]
    [RequireComponent(typeof(UnitDestroy))]
    [RequireComponent(typeof(UnitTriggerMain))]
    public class UnitAttack : MonoBehaviour {
        public class Event {
            public Collider2D otherCollider = null;
            public DamageResult damageResult = null;
            public bool bDamage = false;
        }

        [SerializeField]
        private AttackData _attackData = new AttackData();
        public AttackData attackData { get => _attackData; }

        [SerializeField]
        private bool _bDestroyWhenAttack = false;

        private UnitID _unitID = null;
        private UnitTriggerMain _unitCollider = null;
        private UnitDestroy _unitDestroy = null;

        private SimpleNotify<UnitAttack.Event> _attackNotify = new SimpleNotify<UnitAttack.Event>();
        public SimpleNotify<UnitAttack.Event> attackNotify { get => _attackNotify; }

        private void Awake() {
            _unitID = GetComponent<UnitID>();
            _unitCollider = GetComponent<UnitTriggerMain>();
            _unitDestroy = GetComponent<UnitDestroy>();

            _unitCollider.triggerNotify.AddListener(this, (Collider2D other) => {
                var otherID = other.GetComponent<UnitID>();
                if (!otherID) { return; }
                if (_unitID.campType == otherID.campType) { return; }

                var msg = new UnitAttack.Event();
                msg.otherCollider = other;

                var otherHealth = other.GetComponent<UnitHealth>();
                if (otherHealth) {
                    msg.damageResult = otherHealth.OnDamage(attackData, _unitID);
                }

                msg.bDamage = (msg.damageResult != null);
                _attackNotify.Send(msg);

                if (_bDestroyWhenAttack) {
                    _unitDestroy.InvokeDestroy(DestroyType.None);
                }
            });
        }
    }
}