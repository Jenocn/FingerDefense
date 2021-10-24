using System.Collections.Generic;
using Game.Modules;
using Game.Systems;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Managers {
	public class AudioChannelContainer {
		private static LogSystem.Logger _logger = null;
		public Transform transform { get; private set; }
		private Dictionary<int, AudioSource> _channels = new Dictionary<int, AudioSource>();
		private LinkedList<int> _validCount = new LinkedList<int>();
		private HashSet<int> _lockChannels = new HashSet<int>();
		private AudioMixerGroup _defaultMixerGroup = null;
		private CachePool _cachePool = new CachePool();
		private int _autoChannelCount = 0;
		private bool _bEnableGC = false;

		static AudioChannelContainer() {
			_logger = LogSystem.GetLogger("AudioChannel");
		}

		public AudioChannelContainer(string name, Transform parent, AudioMixerGroup defaultMixerGroup = null, bool bEnableGC = false) {
			var newObj = new GameObject(name);
			transform = newObj.transform;
			transform.SetParent(parent);
			_defaultMixerGroup = defaultMixerGroup;
			_bEnableGC = bEnableGC;
		}

		public void EnableGC(bool bEnabled) {
			_bEnableGC = bEnabled;
		}

		/// <summary>
		/// 新建一个音轨,返回音轨ID
		/// </summary>
		public int NewChannel() {
			if (_bEnableGC && _channels.Count > 50) {
				var removeList = new LinkedList<int>();
				foreach (var item in _channels) {
					if (item.Value) {
						if (item.Value.isPlaying) {
							continue;
						}
						if (_lockChannels.Contains(item.Key)) {
							continue;
						}
						_cachePool.Push(item.Value.gameObject);
					}
					removeList.AddLast(item.Key);
					_validCount.AddLast(item.Key);
				}
				foreach (var key in removeList) {
					_channels.Remove(key);
				}
			}

			var channel = _CountChannel();
			GameObject newObj = null;
			AudioSource audioSource = null;
			if (_cachePool.Empty()) {
				newObj = new GameObject("channel_" + channel);
				newObj.transform.SetParent(transform);
				audioSource = newObj.AddComponent<AudioSource>();
			} else {
				newObj = _cachePool.Pop();
				newObj.name = "channel_" + channel;
				audioSource = newObj.GetComponent<AudioSource>();
			}
			audioSource.outputAudioMixerGroup = _defaultMixerGroup;
			audioSource.playOnAwake = false;
			_channels.Add(channel, audioSource);
			return channel;
		}

		/// <summary>
		/// 锁定channel,不会被释放,如果当前channel不存在则返回false
		/// </summary>
		public bool LockChannel(int channel) {
			if (_channels.ContainsKey(channel)) {
				_lockChannels.Add(channel);
				return true;
			}
			return false;
		}
		public void UnlockChannel(int channel) {
			_lockChannels.Remove(channel);
		}
		public void UnlockAllChannels() {
			_lockChannels.Clear();
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
			foreach (var item in _channels) {
				item.Value.Stop();
			}
		}

		public void RemoveAllChannels() {
			var removeList = new LinkedList<int>();
			foreach (var item in _channels) {
				if (!_lockChannels.Contains(item.Key)) {
					Object.Destroy(item.Value.gameObject);
					removeList.AddLast(item.Key);
				}
			}
			foreach (var item in removeList) {
				_channels.Remove(item);
			}
			_autoChannelCount = 0;
			_cachePool.Clear(true);
			_validCount.Clear();
		}
		private int _CountChannel() {
			if (_validCount.Last != null) {
				var ret = _validCount.Last.Value;
				_validCount.RemoveLast();
				return ret;
			}
			while (_lockChannels.Contains(_autoChannelCount++));
			return _autoChannelCount;
		}

		private AudioSource _GetChannelSource(int channel) {
			if (_channels.TryGetValue(channel, out var source)) {
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