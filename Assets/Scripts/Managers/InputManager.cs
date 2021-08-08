using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Managers {
    public enum InputCommand {
        Up, // 上
        Down, // 下
        Left, // 左
        Right, // 右
        Run, // 跑
        Interaction, // 交互
        MainWeapon, // 主武器
        SubWeapon, // 副武器
        Dodge, // 闪避
        SpecialWeapon, // 特殊武器
        UseItem1, // 使用道具1
        UseItem2, // 使用道具2
        RotateL, // 左转
        RotateR, // 右转
        MenuSwitch, // 菜单开关
        MenuUp, // 菜单上
        MenuDown, // 菜单下
        MenuLeft, // 菜单左
        MenuRight, // 菜单右
        MenuOK, // 菜单确定
        MenuCancel, // 菜单取消
        MiniMap, // 小地图
    }
    public enum AxisID {
        Left, // 左摇杆
        Right, // 右摇杆
        LRT, // LT & RT
    }
    public class InputManager : ManagerBase<InputManager> {

        private static List<string> _buttonList = new List<string>() {
            "ButtonOPTION",
            "ButtonSHARE",
            "ButtonL3",
            "ButtonR3",
            "ButtonLB",
            "ButtonRB",
            "ButtonA",
            "ButtonB",
            "ButtonX",
            "ButtonY",
        };

        private Dictionary<InputCommand, KeyCode> _keyboardBinds = new Dictionary<InputCommand, KeyCode>();
        private Dictionary<InputCommand, string> _evtNameBinds = new Dictionary<InputCommand, string>();
        private Dictionary<InputCommand, System.Tuple<string, float, float>> _axisToBinds = new Dictionary<InputCommand, System.Tuple<string, float, float>>();

        public void RebindCommand(InputCommand command, KeyCode keyCode) {
            if (_keyboardBinds.TryGetValue(command, out var old)) {
                InputSystem.RemoveBind(old, (int) command);
            }
            _keyboardBinds[command] = keyCode;
            InputSystem.AddButtonBind(keyCode, (int) command);
        }
        public void RebindCommand(InputCommand command, string button) {
            if (_evtNameBinds.TryGetValue(command, out var old)) {
                InputSystem.RemoveBind(old, (int) command);
            }
            _evtNameBinds[command] = button;
            InputSystem.AddButtonBind(button, (int) command);
        }
        public void RebindCommand(InputCommand command, string axisName, float min, float max) {
            if (_axisToBinds.TryGetValue(command, out var old)) {
                InputSystem.RemoveBind(old.Item1, old.Item2, old.Item3, (int) command);
            }
            _axisToBinds[command] = new System.Tuple<string, float, float>(axisName, min, max);
            InputSystem.AddButtonBind(axisName, min, max, (int) command);
        }

        public void RebindDefault() {
            // default config
            _keyboardBinds.Clear();
            RebindCommand(InputCommand.Up, KeyCode.W);
            RebindCommand(InputCommand.Down, KeyCode.S);
            RebindCommand(InputCommand.Left, KeyCode.A);
            RebindCommand(InputCommand.Right, KeyCode.D);
            RebindCommand(InputCommand.Run, KeyCode.LeftShift);
            RebindCommand(InputCommand.Interaction, KeyCode.F);
            RebindCommand(InputCommand.MainWeapon, KeyCode.Mouse0);
            RebindCommand(InputCommand.SubWeapon, KeyCode.Mouse1);
            RebindCommand(InputCommand.Dodge, KeyCode.LeftShift);
            RebindCommand(InputCommand.SpecialWeapon, KeyCode.R);
            RebindCommand(InputCommand.UseItem1, KeyCode.Alpha1);
            RebindCommand(InputCommand.UseItem2, KeyCode.Alpha2);
            RebindCommand(InputCommand.RotateL, KeyCode.Q);
            RebindCommand(InputCommand.RotateR, KeyCode.E);
            RebindCommand(InputCommand.MenuSwitch, KeyCode.Escape);
            RebindCommand(InputCommand.MenuUp, KeyCode.W);
            RebindCommand(InputCommand.MenuDown, KeyCode.S);
            RebindCommand(InputCommand.MenuLeft, KeyCode.A);
            RebindCommand(InputCommand.MenuRight, KeyCode.D);
            RebindCommand(InputCommand.MenuOK, KeyCode.Return);
            RebindCommand(InputCommand.MenuCancel, KeyCode.Escape);
            RebindCommand(InputCommand.MiniMap, KeyCode.Tab);

            RebindCommand(InputCommand.Run, "ButtonA");
            RebindCommand(InputCommand.Interaction, "ButtonA");
            RebindCommand(InputCommand.MainWeapon, "AxisLRT", 0.5f, 1.0f);
            RebindCommand(InputCommand.SubWeapon, "ButtonX");
            RebindCommand(InputCommand.SpecialWeapon, "AxisLRT", -1.0f, -0.5f);
            RebindCommand(InputCommand.Dodge, "ButtonB");
            RebindCommand(InputCommand.UseItem1, "MenuLR", -1.0f, -0.5f);
            RebindCommand(InputCommand.UseItem2, "MenuLR", 0.5f, 1.0f);
            RebindCommand(InputCommand.MenuSwitch, "ButtonOPTION");
            RebindCommand(InputCommand.MenuUp, "MenuUD", 0.5f, 1.0f);
            RebindCommand(InputCommand.MenuDown, "MenuUD", -1.0f, -0.5f);
            RebindCommand(InputCommand.MenuLeft, "MenuLR", -1.0f, -0.5f);
            RebindCommand(InputCommand.MenuRight, "MenuLR", 0.5f, 1.0f);
            RebindCommand(InputCommand.MenuOK, "ButtonA");
            RebindCommand(InputCommand.MenuCancel, "ButtonB");
            RebindCommand(InputCommand.MiniMap, "ButtonY");
        }

        public override void OnInitManager() {
            InputSystem.AddAxisBind("Horizontal", "Vertical", (int) AxisID.Left);
            InputSystem.AddAxisBind("HorizontalVIEW", "VerticalVIEW", (int) AxisID.Right);
            InputSystem.AddAxisBind("AxisLRT", "AxisLRT", (int) AxisID.LRT);

            var configStr = ArchiveSystem.common.GetString(this, "input_binds", "");
            if (string.IsNullOrEmpty(configStr)) {
                RebindDefault();
            } else {
                var configReader = GCL.Serialization.INITool.ParseTo(configStr);
                var inputCommandType = typeof(InputCommand);
                var cmdEnumArray = inputCommandType.GetEnumValues();
                foreach (var item in cmdEnumArray) {
                    var enumValue = System.Convert.ToInt32(item);
                    var enumName = inputCommandType.GetEnumName(enumValue);
                    var keyCode = configReader.GetInt("keycode", "cmd_" + enumName, -1);
                    if (keyCode != -1) {
                        RebindCommand((InputCommand) enumValue, (KeyCode) keyCode);
                    }
                    var eventName = configReader.GetString("event", "cmd_" + enumName, "");
                    if (!string.IsNullOrEmpty(eventName)) {
                        RebindCommand((InputCommand) enumValue, eventName);
                    }
                }
            }
        }

        public override void OnArchiveSaveBegin() {
            var configReader = new GCL.Serialization.INIReader();
            var inputCommandType = typeof(InputCommand);
            foreach (var item in _keyboardBinds) {
                var enumName = inputCommandType.GetEnumName(item.Key);
                configReader.Set("keycode", "cmd_" + enumName, (int) item.Value);
            }
            foreach (var item in _evtNameBinds) {
                var enumName = inputCommandType.GetEnumName(item.Key);
                configReader.Set("event", "cmd_" + enumName, item.Value);
            }
            var saveData = GCL.Serialization.INITool.ToString(configReader);
            ArchiveSystem.common.SetString(this, "input_binds", saveData);
        }
    }
}