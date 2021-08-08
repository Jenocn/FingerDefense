using System;
using System.Collections.Generic;
using System.IO;
using GCL.Base;
using GCL.Serialization;

namespace Game.Systems {
	/// <summary>
	/// 存档管理系统
	/// </summary>
	public static class ArchiveSystem {
		private static string _rootPath = "user/savedata";
		private static string _extname = ".dat";
		private static string _archiveNameOfIndex = "archive";
		private static string _commonName = "common";

		private static Dictionary<string, Archive> _archives = new Dictionary<string, Archive>();
		private static Archive _current = null;
		private static Archive _common = null;

		public static Archive common {
			get {
				if (!_common) {
					_common = GetCommonArchive();
				}
				return _common;
			}
		}
		public static Archive current { get => _current; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="rootPath">存档根目录</param>
		/// <param name="extname">存档扩展名</param>
		public static void Init(string rootPath, string extname) {
			_rootPath = rootPath;
			_extname = extname;
		}

		/// <summary>
		/// 设置通过GetArchive(uint index)方法获取存档时的默认名称
		/// <para>当前默认为"archive"</para>
		/// </summary>
		public static void SetArchiveDefaultNameOfIndex(string name) {
			_archiveNameOfIndex = name;
		}

		/// <summary>
		/// 设置通过GetCommonArchive()获取存档时的名称
		/// <para>当前默认为"common"</para>
		/// </summary>
		public static void SetCommonName(string name) {
			_commonName = name;
			_common = GetCommonArchive();
		}

		public static string GetRootPath() {
			return _rootPath;
		}
		public static string GetExtname() {
			return _extname;
		}

		/// <summary>
		/// 设置当前存档
		/// </summary>
		public static void SetCurrent(Archive archive) {
			_current = archive;
		}
		/// <summary>
		/// 获取当前存档,需要预先设置SetCurrent(Archive archive)
		/// </summary>
		public static Archive GetCurrent() {
			return _current;
		}

		/// <summary>
		/// 获取存档
		/// </summary>
		public static Archive GetArchive(string name) {
			if (string.IsNullOrEmpty(name)) {
				return null;
			}
			if (!_archives.TryGetValue(name, out var ret)) {
				ret = Archive.Create(name);
				_archives.Add(name, ret);
			}
			return ret;
		}

		/// <summary>
		/// 提供一个特化版本,通过存档序号获取存档,等价于GetArchive("archive" + index)
		/// </summary>
		/// <param name="index">存档序号</param>
		public static Archive GetArchive(uint index) {
			return GetArchive(_archiveNameOfIndex + index);
		}

		/// <summary>
		/// 获取公共存档数据
		/// </summary>
		public static Archive GetCommonArchive() {
			return GetArchive(_commonName);
		}

		public class Archive {
			public static implicit operator bool(Archive value) {
				return value != null;
			}
			private string _name = "";
			public string Name { get { return _name; } }
			private Dictionary<string, Dictionary<string, object>> _data = new Dictionary<string, Dictionary<string, object>>();

			public static Archive Create(string name) {
				return new Archive(name);
			}

			private Archive(string name) {
				_name = name;
			}

			public void SetInt(object sender, string key, int value) {
				_Set<int>(sender, key, value);
			}
			public int GetInt(object sender, string key, int def) {
				return Convert.ToInt32(_Get<int>(sender, key, def));
			}

			public void SetFloat(object sender, string key, float value) {
				_Set<float>(sender, key, value);
			}
			public float GetFloat(object sender, string key, float def) {
				return Convert.ToSingle(_Get<float>(sender, key, def));
			}

			public void SetDouble(object sender, string key, double value) {
				_Set<double>(sender, key, value);
			}
			public double GetDouble(object sender, string key, double def) {
				return Convert.ToDouble(_Get<double>(sender, key, def));
			}

			public void SetLong(object sender, string key, long value) {
				_Set<long>(sender, key, value);
			}
			public long GetLong(object sender, string key, long def) {
				return Convert.ToInt64(_Get<long>(sender, key, def));
			}

			public void SetBool(object sender, string key, bool value) {
				_Set<bool>(sender, key, value);
			}
			public bool Getbool(object sender, string key, bool def) {
				return Convert.ToBoolean(_Get<bool>(sender, key, def));
			}

			public void SetString(object sender, string key, string value) {
				_Set<string>(sender, key, value);
			}
			public string GetString(object sender, string key, string def) {
				return Convert.ToString(_Get<string>(sender, key, def));
			}

			public void Load() {
				var filename = PathTool.Join(ArchiveSystem.GetRootPath(), Name + ArchiveSystem.GetExtname());
				if (File.Exists(filename)) {
					var data = File.ReadAllText(filename, System.Text.Encoding.UTF8);
					LoadData(data);
				}
			}
			public void Save() {
				SaveData(GetData());
			}
			public void Delete() {
				var filename = PathTool.Join(ArchiveSystem.GetRootPath(), Name + ArchiveSystem.GetExtname());
				if (File.Exists(filename)) {
					File.Delete(filename);
				}
			}

			public void SaveData(string data) {
				var path = ArchiveSystem.GetRootPath();
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				var filename = PathTool.Join(path, Name + ArchiveSystem.GetExtname());
				File.WriteAllText(filename, data, System.Text.Encoding.UTF8);
			}
			public void LoadData(string data) {
				_data = JSONTool.ParseToCustomKV<string, Dictionary<string, object>>(_DecodeSrc(data));
			}
			public string GetData() {
				return _EncodeSrc(JSONTool.ToString(_data));
			}

			private void _Set<T>(object sender, string key, object value) {
				var enSender = _ToSender(sender);
				if (!_data.TryGetValue(enSender, out var tempDict)) {
					tempDict = new Dictionary<string, object>();
					_data.Add(enSender, tempDict);
				}
				_data[enSender][_ToKey<T>(key)] = value;
			}
			private object _Get<T>(object sender, string key, object def) {
				if (_data.TryGetValue(_ToSender(sender), out var tempDict)) {
					if (tempDict.TryGetValue(_ToKey<T>(key), out var ret)) {
						return ret;
					}
				}
				return def;
			}

			private string _ToSender(object sender) {
				return EncryptTool.ToMD5(sender.GetType().ToString());
			}
			private string _ToKey<T>(string key) {
				return EncryptTool.ToMD5(typeof(T).Name + key);
			}
			private string _EncodeSrc(string src) {
				return EncryptTool.EncodeB64R(src);
			}
			private string _DecodeSrc(string src) {
				return EncryptTool.DecodeB64R(src);
			}
		}
	}
}