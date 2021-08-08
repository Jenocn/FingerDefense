using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitBrick : MonoBehaviour {

        [SerializeField]
        private HealthData _health = new HealthData();
        [SerializeField, DisplayOnly]
        private int _curHP = 0;
        public int curHP { get => _curHP; }

        private UnitDestroy _unitDestroy = null;

        private DamageManager _damageMgr = null;

        private void Awake() {
            _unitDestroy = GetComponent<UnitDestroy>();

            _damageMgr = ManagerCenter.GetManager<DamageManager>();
        }

        private void Start() {
            _curHP = _health.hpMax;
        }

        public void OnDamage(AttackData attackData) {
            var damageResult = _damageMgr.CalcDamage(_curHP, attackData);
            _curHP = damageResult.resultHP;
            if (_curHP <= 0) {
                _unitDestroy.InvokeDestroy(DestroyType.Death);
            }
        }
    }
}