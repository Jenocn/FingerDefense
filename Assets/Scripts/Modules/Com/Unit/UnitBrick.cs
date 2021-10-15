using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitID))]
    [RequireComponent(typeof(UnitDestroy))]
    [RequireComponent(typeof(UnitHealth))]
    public class UnitBrick : MonoBehaviour {

        [System.Serializable]
        public class _SpriteBindHP {
            public int hp = 0;
            public Sprite sprite = null;
            public Sprite hurt_sprite = null;
        }

        [SerializeField]
        private List<_SpriteBindHP> _binds = new List<_SpriteBindHP>();
        private _SpriteBindHP _currentBind = null;
        private UnitID _unitID = null;
        private UnitHealth _unitHealth = null;
        public UnitHealth unitHealth { get => _unitHealth; }
        private SpriteRenderer _spriteRenderer = null;
        private DamageManager _damageMgr = null;
        private UnitTriggerSub _triggerSub = null;

        private const float TIME_BLINK = 0.05f;

        private void Awake() {
            _unitID = GetComponent<UnitID>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _unitHealth = GetComponent<UnitHealth>();
            _unitHealth.SetDestroyDelayTime(TIME_BLINK);
            _triggerSub = GetComponent<UnitTriggerSub>();

            _damageMgr = ManagerCenter.GetManager<DamageManager>();

            _unitHealth.damageNotify.AddListener(this, (UnitHealth.Event e) => {
                _UpdateBindSprite(true);
                if (e.result.bDie) {
                    _triggerSub.enabled = false;
                }
                MessageCenter.Send(new MessageBrickHit(
                    _unitID.uniqueID,
                    e.result,
                    transform.position,
                    e.attackID.uniqueID,
                    e.attackID.elementType));
            });
        }

        private void OnDestroy() {
            _unitHealth.damageNotify.RemoveListener(this);
        }

        private void Start() {
            Init();
        }

        public void Init() {
            _triggerSub.enabled = true;
            _unitHealth.ResetCurHP();
            _UpdateBindSprite(false);
        }

        private void _UpdateBindSprite(bool bHurt) {
            if (_spriteRenderer) {
                _SpriteBindHP ret = null;
                foreach (var item in _binds) {
                    if (_unitHealth.curHP == item.hp) {
                        ret = item;
                        break;
                    } else if (_unitHealth.curHP > item.hp) {
                        if (ret == null) {
                            ret = item;
                        } else if (ret.hp < item.hp) {
                            ret = item;
                        }
                    }
                }
                if (ret == null) {
                    ret = _currentBind;
                }
                if (ret != null) {
                    if (bHurt && ret.hurt_sprite) {
                        StartCoroutine(_BeHurt());
                        _spriteRenderer.sprite = (_currentBind != null) ? _currentBind.hurt_sprite : ret.hurt_sprite;
                    } else {
                        _spriteRenderer.sprite = ret.sprite;
                    }
                    _currentBind = ret;
                }
            }
        }

        private IEnumerator _BeHurt() {
            yield return new WaitForSeconds(TIME_BLINK);
            if (_spriteRenderer) {
                if (_currentBind != null) {
                    _spriteRenderer.sprite = _currentBind.sprite;
                }
            }
        }
    }
}