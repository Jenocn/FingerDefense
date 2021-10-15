using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(UnitDestroy))]
    public class UnitEffect : MonoBehaviour {
        private SpriteSequenceAnimation _seqAnima = null;
        private ParticleSystem _particleSys = null;
        private UnitDestroy _unitDestroy = null;

        private void Awake() {
            _unitDestroy = GetComponent<UnitDestroy>();
            _seqAnima = GetComponent<SpriteSequenceAnimation>();
            _seqAnima?.AddEndCallback(this, () => {
                _DestroySelf();
            });

            _particleSys = GetComponent<ParticleSystem>();
            if (_particleSys) {
                var pmain = _particleSys.main;
                pmain.stopAction = ParticleSystemStopAction.Callback;
            }
        }

        private void OnParticleSystemStopped() {
            _DestroySelf();
        }

        private void OnDestroy() {
            _seqAnima?.RemoveEndCallback(this);
        }

        public void Init() {
            _seqAnima?.Replay();
            if (_particleSys) {
                _particleSys.Play();
            }
        }

        private void _DestroySelf() {
            _unitDestroy.InvokeDestroy(DestroyType.None);
        }
    }
}