using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger2D : MonoBehaviour {
        public enum Action {
            None,
            Play,
            Pause,
            Stop,
        }

        [SerializeField]
        private Action _enterAction = Action.None;
        [SerializeField]
        private Action _exitAction = Action.None;

        private AudioSource _audioSource = null;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            _Trigger(_enterAction);
        }
        private void OnTriggerExit2D(Collider2D other) {
            _Trigger(_exitAction);
        }

        private void _Trigger(Action type_) {
            switch (type_) {
            case Action.Play:
                _Play();
                break;
            case Action.Stop:
                _Stop();
                break;
            case Action.Pause:
                _Pause();
                break;
            }
        }

        private void _Play() {
            _audioSource.Play();
        }
        private void _Stop() {
            _audioSource.Stop();
        }
        private void _Pause() {
            _audioSource.Pause();
        }
    }
}