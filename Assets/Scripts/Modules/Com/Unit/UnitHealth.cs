using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitHealth : MonoBehaviour {

        public class Event {
            public Event(DamageResult result, UnitID attackID) {
                this.result = result;
                this.attackID = attackID;
            }
            public DamageResult result { get; private set; } = null;
            public UnitID attackID { get; private set; } = null;
        }

        [SerializeField]
        private HealthData _healthData = new HealthData();
        public HealthData healthData { get => _healthData; }

        [SerializeField, DisplayOnly]
        private int _curHP = 0;
        public int curHP { get => _curHP; }
        private SimpleNotify<UnitHealth.Event> _damageNotify = new SimpleNotify<UnitHealth.Event>();
        public SimpleNotify<UnitHealth.Event> damageNotify { get => _damageNotify; }

        private float _destroyDelaySecond = 0;

        private DamageManager _damageManager = null;
        private UnitDestroy _unitDestroy = null;

        private void Awake() {
            _unitDestroy = GetComponent<UnitDestroy>();
            _damageManager = ManagerCenter.GetManager<DamageManager>();
        }

        private void Start() {
            ResetCurHP();
        }

        public void SetDestroyDelayTime(float sec) {
            _destroyDelaySecond = sec;
        }

        public void ResetCurHP() {
            _curHP = _healthData.hpMax;
        }

        public DamageResult OnDamage(AttackData attackData, UnitID attackID) {
            var damageResult = _damageManager.CalcDamage(_curHP, attackData);
            _curHP = damageResult.resultHP;
            _damageNotify.Send(new UnitHealth.Event(damageResult, attackID));
            if (damageResult.bDie) {
                _unitDestroy.InvokeDestroy(DestroyType.Death, _destroyDelaySecond);
            }
            return damageResult;
        }
    }
}