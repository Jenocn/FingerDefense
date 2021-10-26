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

            VirtualMachine.LocateLogger((string info) => {
                _logger.Log(info);
            }, (string error) => {
                _logger.LogError(error);
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

            VirtualMachine.modulePool.AddModuleFilename("Base", "base.peak");
            VirtualMachine.LoadFile("main.peak")?.Execute();
        }
        private VirtualJourney virtualJourney;
        public override void OnDestroyManager() {}

        private void _InitUnityModule() {
            var space = new peak.interpreter.Space(peak.interpreter.SpaceType.None);
            var module = new peak.interpreter.Module("Unity", space);
            VirtualMachine.modulePool.AddModule("Unity", module);

            // Random
            var objectRandom = new ValueObject();
            module.space.AddVariable(new Variable("Random", VariableAttribute.Const, objectRandom));
            objectRandom.space.AddVariable(new peak.interpreter.Variable("Range", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(2, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    if (ValueTool.IsInteger(args[0]) && ValueTool.IsInteger(args[1])) {
                        return new ValueNumber(Random.Range((int) (args[0] as ValueNumber).value, (int) (args[1] as ValueNumber).value));
                    }
                    _logger.LogError("\"Random.Range()\", the argument isn't Integer!");
                    return ValueNull.DEFAULT_VALUE;
                })
            ));

            // Mathf
            var objectMathf = new ValueObject();
            module.space.AddVariable(new Variable("Mathf", VariableAttribute.Const, objectMathf));
            objectMathf.space.AddVariable(new peak.interpreter.Variable("Clamp", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(3, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    if (ValueTool.IsNumber(args[0]) && ValueTool.IsNumber(args[1]) && ValueTool.IsNumber(args[2])) {
                        return new ValueNumber(Mathf.Clamp(
                            (float) (args[0] as ValueNumber).value,
                            (float) (args[1] as ValueNumber).value,
                            (float) (args[2] as ValueNumber).value));
                    }
                    _logger.LogError("\"Mathf.Clamp()\", the argument isn't Number!");
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            objectMathf.space.AddVariable(new peak.interpreter.Variable("Clamp01", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(1, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    if (ValueTool.IsNumber(args[0])) {
                        return new ValueNumber(Mathf.Clamp01((float) (args[0] as ValueNumber).value));
                    }
                    _logger.LogError("\"Mathf.Clamp01()\", the argument isn't Number!");
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            objectMathf.space.AddVariable(new peak.interpreter.Variable("Max", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(2, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    if (ValueTool.IsNumber(args[0]) && ValueTool.IsNumber(args[1])) {
                        return new ValueNumber(Mathf.Max(
                            (float) (args[0] as ValueNumber).value,
                            (float) (args[1] as ValueNumber).value));
                    }
                    _logger.LogError("\"Mathf.Max()\", the argument isn't Number!");
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            objectMathf.space.AddVariable(new peak.interpreter.Variable("Min", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(2, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    if (ValueTool.IsNumber(args[0]) && ValueTool.IsNumber(args[1])) {
                        return new ValueNumber(Mathf.Min(
                            (float) (args[0] as ValueNumber).value,
                            (float) (args[1] as ValueNumber).value));
                    }
                    _logger.LogError("\"Mathf.Min()\", the argument isn't Number!");
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
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
            // create_ball
            module.space.AddVariable(new peak.interpreter.Variable("create_ball", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(6, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    var msg = new PeakMessage_CreateBall();
                    if (ValueTool.IsInteger(args[0])) {
                        msg.ballID = (int) ((ValueNumber) args[0]).value;
                    }
                    if (ValueTool.IsNumber(args[1]) && ValueTool.IsNumber(args[2])) {
                        msg.position.Set((float) ((ValueNumber) args[1]).value, (float) ((ValueNumber) args[2]).value);
                    }
                    if (ValueTool.IsNumber(args[3]) && ValueTool.IsNumber(args[4])) {
                        msg.direction.Set((float) ((ValueNumber) args[3]).value, (float) ((ValueNumber) args[4]).value);
                    }
                    if (ValueTool.IsNumber(args[5])) {
                        msg.delay = (float) ((ValueNumber) args[5]).value;
                    }
                    message.Send(msg);

                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            // create_brick
            module.space.AddVariable(new peak.interpreter.Variable("create_brick", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(6, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    var msg = new PeakMessage_CreateBrick();
                    if (ValueTool.IsInteger(args[0])) {
                        msg.brickID = (int) ((ValueNumber) args[0]).value;
                    }
                    if (ValueTool.IsNumber(args[1]) && ValueTool.IsNumber(args[2])) {
                        msg.position.Set((float) ((ValueNumber) args[1]).value, (float) ((ValueNumber) args[2]).value);
                    }
                    if (ValueTool.IsInteger(args[3])) {
                        msg.hpMax = (int) ((ValueNumber) args[3]).value;
                    }
                    message.Send(msg);

                    return ValueNull.DEFAULT_VALUE;
                })
            ));

            // music
            module.space.AddVariable(new peak.interpreter.Variable("music", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(4, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    var channelType = MusicChannelType.BGM;
                    string audioName = "";
                    float volume = 1.0f;
                    bool bLoop = true;
                    if (ValueTool.IsInteger(args[0])) {
                        channelType = (MusicChannelType) ((ValueNumber) args[0]).value;
                    }
                    if (ValueTool.IsString(args[1])) {
                        audioName = args[1].ToString();
                    }
                    if (ValueTool.IsNumber(args[2])) {
                        volume = (float) ((ValueNumber) args[2]).value;
                    }
                    if (ValueTool.IsBool(args[3])) {
                        bLoop = ((ValueBool) args[3]).value;
                    }
                    MessageCenter.Send(new MessageMusicPlay(channelType, audioName, volume, bLoop));
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            // music_control
            module.space.AddVariable(new peak.interpreter.Variable("music_control", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(2, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    var channelType = MusicChannelType.BGM;
                    var controlType = AudioControlType.None;
                    if (ValueTool.IsInteger(args[0])) {
                        channelType = (MusicChannelType) ((ValueNumber) args[0]).value;
                    }
                    if (ValueTool.IsInteger(args[1])) {
                        controlType = (AudioControlType) ((ValueNumber) args[1]).value;
                    }
                    MessageCenter.Send(new MessageMusicControl(channelType, controlType));
                    return ValueNull.DEFAULT_VALUE;
                })
            ));
            // sound_effect
            module.space.AddVariable(new peak.interpreter.Variable("sound_effect", peak.interpreter.VariableAttribute.Const,
                new peak.interpreter.ValueFunction(2, (List<peak.interpreter.Value> args, peak.interpreter.Space space) => {
                    string audioName = "";
                    float volume = 1;
                    if (ValueTool.IsString(args[0])) {
                        audioName = args[0].ToString();
                    }
                    if (ValueTool.IsNumber(args[1])) {
                        volume = (float) ((ValueNumber) args[1]).value;
                    }
                    MessageCenter.Send(new MessageSoundEffect(audioName, volume));
                    return ValueNull.DEFAULT_VALUE;
                })
            ));

            VirtualMachine.modulePool.AddModule("Game", module);
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