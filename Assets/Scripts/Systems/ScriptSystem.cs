using System.Collections.Generic;
using peak;
using UnityEngine;

namespace Game.Systems {
	public class ScriptValue {
		public static readonly ScriptValue NULL = new ScriptValue(peak.interpreter.ValueNull.DEFAULT_VALUE);
		public static implicit operator bool(ScriptValue value) {
			return (value != null) && !peak.interpreter.ValueTool.IsNull(value._value);
		}
		private peak.interpreter.Value _value = null;
		public peak.interpreter.Value value { get => _value; }
		public ScriptValue(peak.interpreter.Value value) {
			_value = value ? value : peak.interpreter.ValueNull.DEFAULT_VALUE;
		}
		public ScriptValue(double v) {
			_value = new peak.interpreter.ValueNumber(v);
		}
		public ScriptValue(bool v) {
			_value = new peak.interpreter.ValueBool(v);
		}
		public ScriptValue(string v) {
			_value = new peak.interpreter.ValueString(v);
		}

		public bool GetBool(bool def) {
			if (peak.interpreter.ValueTool.IsBool(_value)) {
				return (_value as peak.interpreter.ValueBool).value;
			}
			return def;
		}
		public double GetNumber(double def) {
			if (peak.interpreter.ValueTool.IsNumber(_value)) {
				return (_value as peak.interpreter.ValueNumber).value;
			}
			return def;
		}
		public string GetString(string def) {
			if (peak.interpreter.ValueTool.IsString(_value)) {
				return (_value as peak.interpreter.ValueString).value;
			}
			return def;
		}
		public bool IsNull() {
			return peak.interpreter.ValueTool.IsNull(_value);
		}
		public bool IsNumber() {
			return peak.interpreter.ValueTool.IsNumber(_value);
		}
		public bool IsBool() {
			return peak.interpreter.ValueTool.IsBool(_value);
		}
		public bool IsString() {
			return peak.interpreter.ValueTool.IsString(_value);
		}
		public bool IsArray() {
			return peak.interpreter.ValueTool.IsArray(_value);
		}
	};

	/// <summary>
	/// 脚本管理系统
	/// </summary>
	public static class ScriptSystem {
		private static LogSystem.Logger _logger = null;

		public static ScriptValue Execute(string filename, string functionName, List<ScriptValue> args) {
			var extname = GCL.Base.PathTool.Extname(filename);
			if (string.IsNullOrEmpty(extname)) {
				filename = filename + ".peak";
			}
			var journey = VirtualMachine.LoadFile(filename);
			if ((journey != null) && journey.Execute()) {
				var tempArgs = new List<peak.interpreter.Value>();
				for (var i = 0; i < args.Count; ++i) {
					tempArgs.Add(args[i] ? args[i].value : ScriptValue.NULL.value);
				}
				return new ScriptValue(journey.ExecuteFunction(functionName, tempArgs));
			}
			return ScriptValue.NULL;
		}
		public static ScriptValue Execute(string filename, string functionName, params ScriptValue[] args) {
			var extname = GCL.Base.PathTool.Extname(filename);
			if (string.IsNullOrEmpty(extname)) {
				filename = filename + ".peak";
			}
			var journey = VirtualMachine.LoadFile(filename);
			if ((journey != null) && journey.Execute()) {
				var tempArgs = new List<peak.interpreter.Value>();
				for (var i = 0; i < args.Length; ++i) {
					tempArgs.Add(args[i] ? args[i].value : ScriptValue.NULL.value);
				}
				return new ScriptValue(journey.ExecuteFunction(functionName, tempArgs));
			}
			return ScriptValue.NULL;
		}
		public static ScriptValue Execute(string filename, string functionName) {
			var extname = GCL.Base.PathTool.Extname(filename);
			if (string.IsNullOrEmpty(extname)) {
				filename = filename + ".peak";
			}
			var journey = VirtualMachine.LoadFile(filename);
			if ((journey != null) && journey.Execute()) {
				return new ScriptValue(journey.ExecuteFunction(functionName));
			}
			return ScriptValue.NULL;
		}

		public static void Init() {
			VirtualMachine.LoadFile("main.peak")?.Execute();
		}

		static ScriptSystem() {
			_logger = LogSystem.GetLogger("Peak");

			VirtualMachine.LocateLogger((string message) => {
				_logger.Log(message);
			});
			VirtualMachine.LocateOpenSrc((string filename) => {
				var asset = AssetSystem.Load<TextAsset>("scripts", filename);
				if (asset) {
					return asset.text;
				}
				return "";
			});

			_InitUnityModule();
			_InitGameModule();

		}

		private static void _InitUnityModule() {
			var space = new peak.interpreter.Space(peak.interpreter.SpaceType.None);
			var module = new peak.interpreter.Module("Unity", space);
			VirtualMachine.modulePool.AddModule("Unity", module, false);
		}
		private static void _InitGameModule() {
			var module = new peak.interpreter.Module("Game", new peak.interpreter.Space(peak.interpreter.SpaceType.None));

			// string_ui
			module.space.AddVariable(new peak.interpreter.Variable("string_ui", peak.interpreter.VariableAttribute.Const,
				new peak.interpreter.ValueFunction(1, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
					var ret = Game.Strings.StringUi.instance.GetElement(peak.interpreter.ValueTool.ToString(args[0]));
					return new peak.interpreter.ValueString(string.IsNullOrEmpty(ret) ? "" : ret);
				})
			));

			// string_talk
			module.space.AddVariable(new peak.interpreter.Variable("string_talk", peak.interpreter.VariableAttribute.Const,
				new peak.interpreter.ValueFunction(1, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
					var ret = Game.Strings.StringTalk.instance.GetElement(peak.interpreter.ValueTool.ToString(args[0]));
					return new peak.interpreter.ValueString(string.IsNullOrEmpty(ret) ? "" : ret);
				})
			));

			// string_story
			module.space.AddVariable(new peak.interpreter.Variable("string_story", peak.interpreter.VariableAttribute.Const,
				new peak.interpreter.ValueFunction(1, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
					var ret = Game.Strings.StringStory.instance.GetElement(peak.interpreter.ValueTool.ToString(args[0]));
					return new peak.interpreter.ValueString(string.IsNullOrEmpty(ret) ? "" : ret);
				})
			));

			VirtualMachine.modulePool.AddModule("Game", module, false);
		}
	}
}