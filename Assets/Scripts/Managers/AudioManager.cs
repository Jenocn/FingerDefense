using System.Collections.Generic;
using Game.Modules;
using Game.Systems;
using GCL.Pattern;
using UnityEngine;

namespace Game.Managers {
	public class AudioManager : ManagerBase<AudioManager> {
		private static LogSystem.Logger _logger = null;
		static AudioManager() {
			_logger = LogSystem.GetLogger("AudioManager");
		}

		private Transform _audioRoot = null;
		private AudioMixerCollection _audioMixerCollection = null;
		private AudioMixerContainer _audioMixerContainer = new AudioMixerContainer();
		private AudioChannelContainer _musicContainer = null;
		private AudioChannelContainer _effectContainer = null;
		private Dictionary<MusicChannelType, int> _musicChannelDict = new Dictionary<MusicChannelType, int>();

		public void PlayMusic(MusicChannelType type, string name, bool bLoop) {
			_musicContainer.Play(_musicChannelDict[type], name, 1, bLoop, true);
		}
		public void PlayMusic(MusicChannelType type, string name, float volume, bool bLoop) {
			_musicContainer.Play(_musicChannelDict[type], name, volume, bLoop, true);
		}
		public void ControlMusic(MusicChannelType channelType, AudioControlType controlType) {
			_musicContainer.Control(_musicChannelDict[channelType], controlType);
		}
		public void PlayEffect(string name, float volume) {
			_effectContainer.NewChannel(name, volume, false, true);
		}

		public override void OnInitManager() {
			var prefab = AssetSystem.Load<GameObject>("prefabs", "AudioManagerObject");
			_audioRoot = Object.Instantiate(prefab).transform;
			Object.DontDestroyOnLoad(_audioRoot);
			_audioMixerCollection = _audioRoot.GetComponent<AudioMixerCollection>();

			_musicContainer = new AudioChannelContainer("MusicContainer", _audioRoot, _audioMixerCollection.musicMixerGroup, false);
			_effectContainer = new AudioChannelContainer("EffectContainer", _audioRoot, _audioMixerCollection.effectMixerGroup, true);

			var musicChannelType = typeof(MusicChannelType);
			var musicChannelValues = musicChannelType.GetEnumValues();
			foreach (var e in musicChannelValues) {
				var channel = _musicContainer.NewChannel();
				_musicChannelDict.Add((MusicChannelType) System.Convert.ToInt32(e), channel);
			}

			MessageCenter.AddListener<MessageMusicPlay>(this, (MessageMusicPlay msg) => {
				PlayMusic(msg.channelType, msg.audioName, msg.volumePercent, msg.bLoop);
			});
			MessageCenter.AddListener<MessageMusicControl>(this, (MessageMusicControl msg) => {
				ControlMusic(msg.channelType, msg.controlType);
			});
			MessageCenter.AddListener<MessageSoundEffect>(this, (MessageSoundEffect msg) => {
				PlayEffect(msg.audioName, msg.volumePercent);
			});
		}

		public override void OnDestroyManager() {
			MessageCenter.RemoveListener<MessageMusicPlay>(this);
			MessageCenter.RemoveListener<MessageMusicControl>(this);
			MessageCenter.RemoveListener<MessageSoundEffect>(this);
		}

		public override void OnStartManager() {
			_audioMixerContainer.AddMixer("Main", _audioMixerCollection.mainMixerGroup.audioMixer);
			_audioMixerContainer.AddMixer("Music", _audioMixerCollection.musicMixerGroup.audioMixer);
			_audioMixerContainer.AddMixer("Effect", _audioMixerCollection.effectMixerGroup.audioMixer);
			_audioMixerContainer.AddMixer("Voice", _audioMixerCollection.voiceMixerGroup.audioMixer);

			_audioMixerContainer.LoadData();
		}

		public override void OnSceneUnloaded() {
			_effectContainer.RemoveAllChannels();
			_musicContainer.StopAllChannels();
		}

		public override void OnCommonArchiveSaveBegin() {
			_audioMixerContainer.SaveData();
		}
	}
}