using System.Collections.Generic;
using Game.Systems;
using GCL.Pattern;
using peak;
using peak.interpreter;
using UnityEngine;

namespace Game.Managers {

    /// <summary>
    /// Peak脚本管理器
    /// </summary>
    public class ScriptManager : ManagerBase<ScriptManager> {
        private LogSystem.Logger _logger = null;
        private MessageDispatcher _messageDispatcher = new MessageDispatcher();
        private Dictionary<string, VirtualJourney> _cacheVJ = new Dictionary<string, VirtualJourney>();

        public MessageDispatcher message { get => _messageDispatcher; }

        public VirtualJourney Load(string filename) {
            var extname = GCL.Base.PathTool.Extname(filename);
            if (string.IsNullOrEmpty(extname)) {
                filename = filename + ".peak";
            }
            return VirtualMachine.LoadFile(filename);
        }

        public VirtualJourney LoadWithCache(string filename) {
            if (!_cacheVJ.TryGetValue(filename, out var jour)) {
                jour = Load(filename);
                if (jour != null) {
                    _cacheVJ.Add(filename, jour);
                }
            }
            return jour;
        }

        public void RemoveAllCache() {
            _cacheVJ.Clear();
        }

        public void RemoveCache(string filename) {
            _cacheVJ.Remove(filename);
        }

        public ScriptValue Execute(string filename, string functionName, List<ScriptValue> args) {
            return Execute(Load(filename), functionName, args);
        }
        public ScriptValue Execute(string filename, string functionName, params ScriptValue[] args) {
            return Execute(Load(filename), functionName, args);
        }
        public ScriptValue Execute(string filename, string functionName) {
            return Execute(Load(filename), functionName);
        }

        public ScriptValue ExecuteWithCache(string filename, string functionName, List<ScriptValue> args) {
            return Execute(LoadWithCache(filename), functionName, args);
        }
        public ScriptValue ExecuteWithCache(string filename, string functionName, params ScriptValue[] args) {
            return Execute(LoadWithCache(filename), functionName, args);
        }
        public ScriptValue ExecuteWithCache(string filename, string functionName) {
            return Execute(LoadWithCache(filename), functionName);
        }

        public ScriptValue Execute(VirtualJourney journey, string functionName, List<ScriptValue> args) {
            if ((journey != null) && journey.Execute()) {
                var tempArgs = new List<peak.interpreter.Value>();
                for (var i = 0; i < args.Count; ++i) {
                    tempArgs.Add(args[i] ? args[i].value : ScriptValue.NULL.value);
                }
                return new ScriptValue(journey.ExecuteFunction(functionName, tempArgs));
            }
            return ScriptValue.NULL;
        }
        public ScriptValue Execute(VirtualJourney journey, string functionName, params ScriptValue[] args) {
            if ((journey != null) && journey.Execute()) {
                var tempArgs = new List<peak.interpreter.Value>();
                for (var i = 0; i < args.Length; ++i) {
                    tempArgs.Add(args[i] ? args[i].value : ScriptValue.NULL.value);
                }
                return new ScriptValue(journey.ExecuteFunction(functionName, tempArgs));
            }
            return ScriptValue.NULL;
        }
        public ScriptValue Execute(VirtualJourney journey, string functionName) {
            if ((journey != null) && journey.Execute()) {
                return new ScriptValue(journey.ExecuteFunction(functionName));
            }
            return ScriptValue.NULL;
        }

        public override void OnInitManager() {
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

            VirtualMachine.LoadFile("main.peak")?.Execute();
        }
        public override void OnDestroyManager() {}

        private void _InitUnityModule() {
            var space = new peak.interpreter.Space(peak.interpreter.SpaceType.None);
            var module = new peak.interpreter.Module("Unity", space);
            VirtualMachine.modulePool.AddModule("Unity", module, false);
        }
        private void _InitGameModule() {
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

            // create_effect
            module.space.AddVariable(new peak.interpreter.Variable("create_effect", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(4, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {

                    var msg = new PeakMessage_CreateEffect();
                    if (ValueTool.IsInteger(args[0])) {
                        msg.effectID = (int) ((ValueNumber) args[0]).value;
                    }
                    if (ValueTool.IsNumber(args[1]) && ValueTool.IsNumber(args[2])) {
                        msg.position.Set((float) ((ValueNumber) args[1]).value, (float) ((ValueNumber) args[2]).value);
                    }
                    if (ValueTool.IsNumber(args[3])) {
                        msg.delay = (float) ((ValueNumber) args[3]).value;
                    }
                    message.Send(msg);

                    return ValueNull.DEFAULT_VALUE;
                })
            ));

            VirtualMachine.modulePool.AddModule("Game", module, false);
        }
    }

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
    }
}