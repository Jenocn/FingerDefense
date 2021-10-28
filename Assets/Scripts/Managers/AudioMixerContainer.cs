using System.Collections.Generic;
using Game.Modules;
using Game.Systems;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Managers {
	public class AudioMixerContainer {
		public class _Data {
			public AudioMixer mixer { get; set; } = null;
			public float volume { get; set; } = 1;
			public float pitch { get; set; } = 1;
		}

		private Dictionary<string, _Data> _datas = new Dictionary<string, _Data>();

		public void AddMixer(string key, AudioMixer mixer) {
			var data = new _Data();
			data.mixer = mixer;
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
				_SetVolume(data, volume);
			}
		}

		public float GetVolume(string key) {
			if (_datas.TryGetValue(key, out var data)) {
				return data.volume;
			}
			return 0;
		}

		/// <summary>
		/// 0.01 ~ 10, 默认值1
		/// </summary>
		public void SetPitch(string key, float pitch = 1) {
			if (_datas.TryGetValue(key, out var data)) {
				_SetPitch(data, pitch);
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

		public void LoadData(ArchiveSystem.Archive archive) {
			foreach (var item in _datas) {
				_SetVolume(item.Value, archive.GetFloat("AudioMixerContainer", item.Key + "Volume", 1));
				_SetPitch(item.Value, archive.GetFloat("AudioMixerContainer", item.Key + "Pitch", 1));
			}
		}
		public void SaveData(ArchiveSystem.Archive archive) {
			foreach (var item in _datas) {
				archive.SetFloat("AudioMixerContainer", item.Key + "Volume", item.Value.volume);
				archive.SetFloat("AudioMixerContainer", item.Key + "Pitch", item.Value.pitch);
			}
		}

		private void _SetVolume(_Data data, float volume) {
			data.volume = Mathf.Clamp01(volume);
			var dt = 1 - data.volume;
			var v = (1 - dt * dt) * 80 - 80;
			data.mixer.SetFloat("volume", v);
		}
		private void _SetPitch(_Data data, float pitch) {
			data.pitch = Mathf.Clamp(pitch, 0.01f, 10);
			data.mixer.SetFloat("pitch", data.pitch);
		}
	}
}