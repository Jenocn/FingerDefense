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
		public static LogSystem.Logger logger { get; private set; } = null;
		private static string _rootPath = "user/savedata";
		private static string _extname = ".dat";
		private static string _archiveNameOfIndex = "archive";
		private static string _commonName = "common";

		private static Dictionary<string, Archive> _archives = new Dictionary<string, Archive>();
		private static Archive _current = null;
		private static Archive _common = null;
		private static bool _bEncodeKey = true;
		private static bool _bEncodeSrc = true;

		public static Archive common {
			get {
				if (!_common) {
					_common = GetCommonArchive();
				}
				return _common;
			}
		}
		public static Archive current { get => _current; }

		static ArchiveSystem() {
			logger = LogSystem.GetLogger("Archive");
		}

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
		/// 是否加密
		/// </summary>
		/// <param name="bKey">是否加密Key</param>
		/// <param name="bSrc">是否加密整个存档文本</param>
		public static void SetEncodeEnabled(bool bKey, bool bSrc) {
			_bEncodeKey = bKey;
			_bEncodeSrc = bSrc;
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

		private static string _EncodeSession(string session) {
			if (_bEncodeKey) {
				return EncryptTool.ToMD5(session);
			} else {
				return session;
			}
		}
		private static string _EncodeKey<T>(string key) {
			if (_bEncodeKey) {
				return EncryptTool.ToMD5(typeof(T).Name + key);
			} else {
				return typeof(T).Name + "_" + key;
			}
		}
		private static string SIGN_ENCODE = "_E_N_C_O_D_E_";
		private static string _EncodeSrc(string src) {
			if (_bEncodeSrc) {
				return SIGN_ENCODE + EncryptTool.EncodeB64R(src);
			} else {
				return src;
			}
		}
		private static string _DecodeSrc(string src) {
			if (src.Length >= SIGN_ENCODE.Length) {
				var head = src.Substring(0, SIGN_ENCODE.Length);
				if (head == SIGN_ENCODE) {
					src = src.Substring(SIGN_ENCODE.Length);
					return EncryptTool.DecodeB64R(src);
				}
			}
			return src;
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

			public void SetInt(string session, string key, int value) {
				_Set<int>(session, key, value);
			}
			public int GetInt(string session, string key, int def) {
				return Convert.ToInt32(_Get<int>(session, key, def));
			}

			public void SetFloat(string session, string key, float value) {
				_Set<float>(session, key, value);
			}
			public float GetFloat(string session, string key, float def) {
				return Convert.ToSingle(_Get<float>(session, key, def));
			}

			public void SetDouble(string session, string key, double value) {
				_Set<double>(session, key, value);
			}
			public double GetDouble(string session, string key, double def) {
				return Convert.ToDouble(_Get<double>(session, key, def));
			}

			public void SetLong(string session, string key, long value) {
				_Set<long>(session, key, value);
			}
			public long GetLong(string session, string key, long def) {
				return Convert.ToInt64(_Get<long>(session, key, def));
			}

			public void SetBool(string session, string key, bool value) {
				_Set<bool>(session, key, value);
			}
			public bool GetBool(string session, string key, bool def) {
				return Convert.ToBoolean(_Get<bool>(session, key, def));
			}

			public void SetString(string session, string key, string value) {
				_Set<string>(session, key, value);
			}
			public string GetString(string session, string key, string def) {
				return Convert.ToString(_Get<string>(session, key, def));
			}

			public void Load() {
				var filename = PathTool.Join(ArchiveSystem.GetRootPath(), Name + ArchiveSystem.GetExtname());
				if (File.Exists(filename)) {
					var data = File.ReadAllText(filename, System.Text.Encoding.UTF8);
					ReadData(data);
					logger.Log(Name + " loaded!");
				}
			}
			public void Save() {
				WriteData(_data);
				logger.Log(Name + " saved!");
			}
			public void Delete() {
				var filename = PathTool.Join(ArchiveSystem.GetRootPath(), Name + ArchiveSystem.GetExtname());
				if (File.Exists(filename)) {
					File.Delete(filename);
				}
			}

			public void WriteData(Dictionary<string, Dictionary<string, object>> data) {
				var src = _EncodeSrc(JSONTool.ToString(data));
				var path = ArchiveSystem.GetRootPath();
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				var filename = PathTool.Join(path, Name + ArchiveSystem.GetExtname());
				File.WriteAllText(filename, src, System.Text.Encoding.UTF8);
			}
			public void ReadData(string data) {
				_data = JSONTool.ParseToCustomKV<string, Dictionary<string, object>>(_DecodeSrc(data));
			}
			public Dictionary<string, Dictionary<string, object>> GetData() {
				return _data;
			}

			private void _Set<T>(string session, string key, object value) {
				var enSession = _EncodeSession(session);
				if (!_data.TryGetValue(enSession, out var tempDict)) {
					tempDict = new Dictionary<string, object>();
					_data.Add(enSession, tempDict);
				}
				_data[enSession][_EncodeKey<T>(key)] = value;
			}
			private object _Get<T>(string session, string key, object def) {
				if (_data.TryGetValue(_EncodeSession(session), out var tempDict)) {
					if (tempDict.TryGetValue(_EncodeKey<T>(key), out var ret)) {
						return ret;
					}
				}
				return def;
			}
		}
	}
}