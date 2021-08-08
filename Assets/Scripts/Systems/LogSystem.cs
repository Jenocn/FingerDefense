using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems {
	/// <summary>
	/// 日志系统,统一管理日志
	/// </summary>
	public static class LogSystem {
		private static Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
		private static Dictionary<int, System.Action<string, string>> _debugLogCallbacks = new Dictionary<int, Action<string, string>>();
		private static bool _bEnabled = true;
		/// <summary>
		/// 是否开启
		/// </summary>
		public static bool Enabled { get { return _bEnabled; } }
		/// <summary>
		/// 默认的日志器
		/// GetDefaultLogger
		/// </summary>
		/// <returns></returns>
		public static Logger defaultLogger { get { return GetDefaultLogger(); } }

		/// <summary>
		/// 设置日志总开关开启状态
		/// </summary>
		public static void SetEnabled(bool e) {
			_bEnabled = e;
		}

		public static void Init() {
			Application.logMessageReceived += _DebugLogCallback;
		}
		private static void _DebugLogCallback(string condition, string stackTrace, LogType type) {
			foreach (var item in _debugLogCallbacks) {
				item.Value.Invoke(condition, stackTrace);
			}
		}

		public static void RegisterDebugLog(object sender, System.Action<string, string> action) {
			if (!_debugLogCallbacks.ContainsKey(sender.GetHashCode())) {
				_debugLogCallbacks.Add(sender.GetHashCode(), action);
			}
		}
		public static void UnregisterDebugLog(object sender) {
			_debugLogCallbacks.Remove(sender.GetHashCode());
		}

		/// <summary>
		/// 获得一个日志器
		/// </summary>
		/// <param name="name">名称</param>
		public static Logger GetLogger(string name) {
			if (!_loggers.TryGetValue(name, out var logger)) {
				logger = new Logger(name);
				_loggers.Add(name, logger);
			}
			return logger;
		}
		/// <summary>
		/// 获得默认的日志器
		/// </summary>
		public static Logger GetDefaultLogger() {
			return GetLogger("Default");
		}

		public class Logger {
			public string Name { get { return _name; } }
			private string _name = "";
			private bool _bEnabled = true;
			public Logger(string name) {
				_name = name;
				SetEnabled(true);
			}
			/// <summary>
			/// 设置是否开启
			/// </summary>
			public void SetEnabled(bool e) {
				_bEnabled = e;
			}
			/// <summary>
			/// Log当前时间
			/// </summary>
			public void LogTime() {
				LogTime("");
			}
			/// <summary>
			/// Log带当前时间的消息
			/// </summary>
			public void LogTime(object message) {
				Log(string.Format("[{0:HH:mm:ss}] {1}", DateTime.Now, message));
			}
			/// <summary>
			/// Log当前日期
			/// </summary>
			public void LogDate() {
				LogDate("");
			}
			/// <summary>
			/// Log带当前日期的消息
			/// </summary>
			public void LogDate(object message) {
				Log(string.Format("[{0}] {1}", DateTime.Now.ToShortDateString(), message));
			}
			/// <summary>
			/// Log消息
			/// </summary>
			/// <param name="message"></param>
			public void Log(object message) {
				if (!LogSystem.Enabled || !_bEnabled) { return; }
				Debug.Log(string.Format("[{0}] {1}", _name, message.ToString()));
			}
			/// <summary>
			/// Log错误消息
			/// </summary>
			public void LogError(object message) {
				if (!LogSystem.Enabled || !_bEnabled) { return; }
				Debug.LogError(string.Format("[{0}] {1}", _name, message.ToString()));
			}
		}
	}
}