using System.Collections.Generic;
using Game.Systems;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Managers {
	public class AudioChannelContainer {
		private static LogSystem.Logger _logger = null;
		public Transform transform { get; private set; }
		private Dictionary<int, AudioSource> _autoChannels = new Dictionary<int, AudioSource>();
		private int _autoChannelCount = 0;
		private AudioMixerGroup _defaultMixerGroup = null;

		static AudioChannelContainer() {
			_logger = LogSystem.GetLogger("AudioChannel");
		}

		public AudioChannelContainer(string name, Transform parent, AudioMixerGroup defaultMixerGroup = null) {
			var newObj = new GameObject(name);
			transform = newObj.transform;
			transform.SetParent(parent);
			_defaultMixerGroup = defaultMixerGroup;
		}

		/// <summary>
		/// 新建一个音轨,返回音轨ID
		/// </summary>
		public int NewChannel() {
			var channel = _CountChannel();
			var newObj = new GameObject("channel_" + channel);
			newObj.transform.SetParent(transform);
			var audioSource = newObj.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = _defaultMixerGroup;
			audioSource.playOnAwake = false;
			_autoChannels.Add(channel, audioSource);
			return channel;
		}

		/// <summary>
		/// 新建一个音轨, 创建成功返回音轨ID
		/// </summary>
		public int NewChannel(string audioName, float volume, bool bLoop, bool bPlayNow) {
			int channel = NewChannel();
			Play(channel, audioName, volume, bLoop, bPlayNow);
			return channel;
		}

		/// <summary>
		/// 使用已存在的Channel,成功则返回true,否则false
		/// </summary>
		public bool Play(int channel, string audioName, float volume, bool bLoop, bool bPlayNow) {
			var source = _GetChannelSource(channel);
			if (!source) {
				return false;
			}

			var clip = _LoadClip(audioName);
			if (!clip) {
				return false;
			}

			source.clip = clip;
			source.volume = volume;
			source.loop = bLoop;
			if (bPlayNow) {
				source.Play();
			}

			return true;
		}

		public bool Control(int channel, AudioControlType controlType) {
			var source = _GetChannelSource(channel);
			if (!source) {
				return false;
			}

			if (source.clip) {
				switch (controlType) {
				case AudioControlType.Pause:
					source.Pause();
					break;
				case AudioControlType.Play:
					var time = source.time;
					source.Play();
					source.time = time;
					break;
				case AudioControlType.Replay:
					source.time = 0;
					source.Play();
					break;
				case AudioControlType.Stop:
					source.Stop();
					break;
				case AudioControlType.UnPause:
					source.UnPause();
					break;
				}
			}

			return true;
		}

		public bool SetMixer(int channel, AudioMixerGroup mixerGroup) {
			var source = _GetChannelSource(channel);
			if (!source) {
				return false;
			}
			if (mixerGroup) {
				source.outputAudioMixerGroup = mixerGroup;
			} else {
				source.outputAudioMixerGroup = _defaultMixerGroup;
			}
			return true;
		}

		public void StopAllChannels() {
			foreach (var item in _autoChannels) {
				item.Value.Stop();
			}
		}

		public void RemoveAllChannels() {
			_autoChannels.Clear();
			for (var i = 0; i < transform.childCount; ++i) {
				var child = transform.GetChild(i);
				if (child) {
					Object.Destroy(child.gameObject);
				}
			}
			_autoChannelCount = 0;
		}
		private int _CountChannel() {
			return _autoChannelCount++;
		}

		private AudioSource _GetChannelSource(int channel) {
			if (_autoChannels.TryGetValue(channel, out var source)) {
				return source;
			}
			return null;
		}

		private AudioClip _LoadClip(string audioName) {
			var clip = AssetSystem.Load<AudioClip>("audio", audioName);
			if (!clip) {
				_logger.LogError(audioName + " is not found!");
			}
			return clip;
		}
	}
}