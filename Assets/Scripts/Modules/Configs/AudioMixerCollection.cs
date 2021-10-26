using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Modules {
    public class AudioMixerCollection : MonoBehaviour {
        [SerializeField]
        private AudioMixerGroup _mainMixerGroup = null;
        [SerializeField]
        private AudioMixerGroup _musicMixerGroup = null;
        [SerializeField]
        private AudioMixerGroup _effectMixerGroup = null;
        [SerializeField]
        private AudioMixerGroup _voiceMixerGroup = null;

        public AudioMixerGroup mainMixerGroup => _mainMixerGroup;
        public AudioMixerGroup musicMixerGroup => _musicMixerGroup;
        public AudioMixerGroup effectMixerGroup => _effectMixerGroup;
        public AudioMixerGroup voiceMixerGroup => _voiceMixerGroup;
    }
}