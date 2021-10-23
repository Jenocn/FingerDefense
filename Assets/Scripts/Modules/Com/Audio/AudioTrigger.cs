using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(Collider))]
    public class AudioTrigger : MonoBehaviour {
        public enum Type {
            None,
            Play,
            Pause,
            Stop,
        }

        [SerializeField]
        private Type _enterType = Type.None;
        [SerializeField]
        private Type _exitType = Type.None;

        private AudioSource _audioSource = null;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other) {
            _Trigger(_enterType);
        }
        private void OnTriggerExit(Collider other) {
            _Trigger(_exitType);
        }

        private void _Trigger(Type type_) {
            switch (type_) {
            case Type.Play:
                _Play();
                break;
            case Type.Stop:
                _Stop();
                break;
            case Type.Pause:
                _Pause();
                break;
            }
        }

        private void _Play() {
            if (_audioSource) {
                _audioSource.Play();
            }
        }
        private void _Stop() {
            if (_audioSource) {
                _audioSource.Stop();
            }
        }
        private void _Pause() {
            if (_audioSource) {
                _audioSource.Pause();
            }
        }
    }
}