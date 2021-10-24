using System.Collections.Generic;
using Game.Modules;
using Game.Systems;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Managers {
	public class AudioMixerContainer {
		public class Data {
			public AudioMixer mixer = null;
			public float volume = 1;
			public float pitch = 1;
		}

		private Dictionary<string, Data> _datas = new Dictionary<string, Data>();

		public void AddMixer(string key, AudioMixer mixer) {
			var data = new Data();
			data.mixer = mixer;
			data.volume = 1;
			_datas.Add(key, data);
		}
		public void RemoveMixer(string key) {
			_datas.Remove(key);
		}

		/// <summary>
		/// volume (0 ~ 1.0)
		/// </summary>
		public void SetVolume(string key, float volume = 1) {
			if (_datas.TryGetValue(key, out var data)) {
				data.volume = volume;
				data.mixer.SetFloat("volume", Mathf.Clamp01(volume) * 80 - 80);
			}
		}

		public float GetVolume(string key) {
			if (_datas.TryGetValue(key, out var data)) {
				return data.volume;
			}
			return 0;
		}

		/// <summary>
		/// 1 ~ 1000, 默认值100
		/// </summary>
		public void SetPitch(string key, float pitch = 100) {
			if (_datas.TryGetValue(key, out var data)) {
				data.pitch = pitch;
				data.mixer.SetFloat("pitch", Mathf.Clamp(pitch, 1, 1000));
			}
		}

		public float GetPitch(string key) {
			if (_datas.TryGetValue(key, out var data)) {
				return data.pitch;
			}
			return 0;
		}

		public bool Contains(string key) {
			return _datas.ContainsKey(key);
		}

		public void LoadData() {
			foreach (var item in _datas) {
				var data = item.Value;
				data.volume = ArchiveSystem.common.GetFloat("AudioMixerContainer", item.Key + "Volume", 1);
				data.mixer.SetFloat("volume", Mathf.Clamp01(data.volume) * 80 - 80);

				data.volume = ArchiveSystem.common.GetFloat("AudioMixerContainer", item.Key + "Pitch", 100);
				data.mixer.SetFloat("pitch", Mathf.Clamp(data.pitch, 1, 1000));
			}
		}
		public void SaveData() {
			foreach (var item in _datas) {
				ArchiveSystem.common.SetFloat("AudioMixerContainer", item.Key + "Volume", item.Value.volume);
				ArchiveSystem.common.SetFloat("AudioMixerContainer", item.Key + "Pitch", item.Value.pitch);
			}
		}
	}
}