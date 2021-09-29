using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    [RequireComponent(typeof(UnitHealth))]
    public class UnitBrick : MonoBehaviour {

        [System.Serializable]
        public class _SpriteBindHP {
            public int hp = 0;
            public Sprite sprite = null;
        }

        [SerializeField]
        private List<_SpriteBindHP> _binds = new List<_SpriteBindHP>();
        private UnitHealth _unitHealth = null;
        private SpriteRenderer _spriteRenderer = null;
        private DamageManager _damageMgr = null;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _unitHealth = GetComponent<UnitHealth>();

            _damageMgr = ManagerCenter.GetManager<DamageManager>();

            _unitHealth.damageNotify.AddListener(this, (DamageResult result) => {
                if (!result.bDie) {
                    _UpdateBindSprite();
                }
            });
        }

        private void OnDestroy() {
            _unitHealth.damageNotify.RemoveListener(this);
        }

        private void Start() {
            _UpdateBindSprite();
        }

        private void _UpdateBindSprite() {
            if (_spriteRenderer) {
                var ret = _binds.Find((_SpriteBindHP item) => {
                    return item.hp == _unitHealth.curHP;
                });
                if (ret != null) {
                    _spriteRenderer.sprite = ret.sprite;
                }
            }
        }
    }
}