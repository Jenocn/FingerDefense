using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitHealth : MonoBehaviour {
        [SerializeField]
        private HealthData _healthData = new HealthData();
        public HealthData healthData { get => _healthData; }

        [SerializeField, DisplayOnly]
        private int _curHP = 0;
        public int curHP { get => _curHP; }
        private SimpleNotify<DamageResult> _damageNotify = new SimpleNotify<DamageResult>();
        public SimpleNotify<DamageResult> damageNotify { get => _damageNotify; }

        private DamageManager _damageManager = null;
        private UnitDestroy _unitDestroy = null;

        private void Awake() {
            _unitDestroy = GetComponent<UnitDestroy>();
            _damageManager = ManagerCenter.GetManager<DamageManager>();
        }

        private void Start() {
            _curHP = _healthData.hpMax;
        }

        public DamageResult OnDamage(AttackData attackData) {
            var damageResult = _damageManager.CalcDamage(_curHP, attackData);
            _curHP = damageResult.resultHP;
            _damageNotify.Send(damageResult);
            if (damageResult.bDie) {
                _unitDestroy.InvokeDestroy(DestroyType.Death);
            }
            return damageResult;
        }
    }
}