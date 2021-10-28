using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Modules;
using Game.Systems;
using Game.Tables;
using Game.Utility;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiEditorMap : UiModel {

        public class _Data {
            public int index = 0;
            public Vector2 position = Vector2.zero;
            public GameObject handleObject = null;
            public int id = 0;
            public int hpMax = 0;
        }

        private Vector2Int _grid = Vector2Int.one;
        private Vector2 _gridSize = Vector2.zero;

        private Vector2 _min = Vector2.zero;
        private Vector2 _max = Vector2.zero;
        private Rect _contentRect = new Rect();
        private Vector2 _size = Vector2.zero;

        private Dictionary<int, _Data> _dataMap = new Dictionary<int, _Data>();

        private InputField _inputFilename = null;
        private InputField _inputTypeID = null;
        private InputField _inputHp = null;

        public override void OnInitUI() {
            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiEditorMap");
            var ui = InstantiateUI(prefab).transform;

            ui.Find("ButtonOpen")?.GetComponent<Button>().onClick.AddListener(() => {
                _LoadFromFile(_inputFilename.text);
            });
            ui.Find("ButtonSave")?.GetComponent<Button>().onClick.AddListener(() => {
                _SaveToFile(_inputFilename.text);
            });
            ui.Find("ButtonClear")?.GetComponent<Button>().onClick.AddListener(() => {
                _Clear();
            });
            _inputFilename = ui.Find("InputFieldFilename")?.GetComponent<InputField>();
            _inputTypeID = ui.Find("InputFieldTypeID")?.GetComponent<InputField>();
            _inputHp = ui.Find("InputFieldHp")?.GetComponent<InputField>();

            _min = Camera.main.ScreenToWorldPoint(Vector3.zero);
            _max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            _size = _max - _min;
            _contentRect.Set(_min.x, _min.y + _size.y * 0.5f, _size.x, _size.y * 0.5f);
            _Clear();
        }

        private void Update() {
            // create
            bool button0 = Input.GetMouseButton(0);
            // remove
            bool button1 = Input.GetMouseButton(1);
            if (button0 || button1) {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_contentRect.Contains(pos)) {
                    pos.z = 0;
                    var data = _GetDataFromPosition(pos);
                    if (null == data) {
                        return;
                    }
                    if (data.handleObject) {
                        if (button1) {
                            BrickFactory.Delete(data.handleObject);
                            _RemoveData(data);
                            return;
                        }
                    } else {
                        if (button0) {
                            if (int.TryParse(_inputTypeID.text.Trim(), out var typeID)) {
                                data.id = typeID;
                                if (int.TryParse(_inputHp.text.Trim(), out var hpMax)) {
                                    data.hpMax = hpMax;
                                } else {
                                    data.hpMax = 1;
                                }
                                data.handleObject = BrickFactory.Create(data.id, data.hpMax, data.position, null)?.gameObject;
                            }
                        }
                    }
                }
            }
        }

        private void OnRenderObject() {
            var begin = Vector3.zero;
            var end = Vector3.zero;
            for (int i = 0; i < _grid.x - 1; ++i) {
                float x = _min.x + _gridSize.x * (i + 1);
                begin.Set(x, _min.y, transform.position.z);
                end.Set(x, _max.y, transform.position.z);
                UnityUtility.DrawLine(begin, end, Color.white);
            }
            for (int i = 0; i < _grid.y - 1; ++i) {
                float y = _min.y + _gridSize.y * (i + 1);
                begin.Set(_min.x, y, transform.position.z);
                end.Set(_max.x, y, transform.position.z);

                if (i % 4 == 0) {
                    UnityUtility.DrawLine(begin, end, Color.green);
                } else {
                    UnityUtility.DrawLine(begin, end, Color.white);
                }
            }
            begin.Set(Mathf.Lerp(_min.x, _max.x, 0.5f), _min.y, transform.position.z);
            end.Set(Mathf.Lerp(_min.x, _max.x, 0.5f), _max.y, transform.position.z);
            UnityUtility.DrawLine(begin, end, Color.green);
        }

        private _Data _GetDataFromPosition(Vector2 pos) {
            if (_grid.x == 0 || _grid.y == 0) {
                return null;
            }
            if (_gridSize.x == 0 || _gridSize.y == 0) {
                return null;
            }

            int x = Mathf.FloorToInt((pos.x - _min.x) / _gridSize.x);
            int y = Mathf.FloorToInt((pos.y - _min.y) / _gridSize.y);

            var posX = _gridSize.x * x + _min.x + _gridSize.x * 0.5f;
            var posY = _gridSize.y * y + _min.y + _gridSize.y * 0.5f;

            int index = y * _grid.x + x;;

            if (!_dataMap.TryGetValue(index, out var data)) {
                data = new _Data();
                data.index = index;
                data.position.Set(posX, posY);
                _dataMap.Add(index, data);
            }

            return data;
        }

        private void _RemoveData(_Data data) {
            _dataMap.Remove(data.index);
        }

        private void _Clear() {
            foreach (var item in _dataMap) {
                if (item.Value.handleObject) {
                    BrickFactory.Delete(item.Value.handleObject);
                }
            }
            _dataMap.Clear();

            _grid.Set(20, 34);

            _gridSize.x = _size.x / _grid.x;
            _gridSize.y = _size.y / _grid.y;
        }

        private void _ClearOutOfContent() {
            var removeList = new LinkedList<int>();
            foreach (var item in _dataMap) {
                if (!_contentRect.Contains(item.Value.position)) {
                    BrickFactory.Delete(item.Value.handleObject);
                    removeList.AddLast(item.Key);
                }
            }
            foreach (var item in removeList) {
                _dataMap.Remove(item);
            }
        }

        private void _LoadFromFile(string filename) {
            _Clear();
            var absFilename = DIRECTORY_MAPDATA + filename + ".json";
            if (!File.Exists(absFilename)) {
                return;
            }
            var src = File.ReadAllText(absFilename);
            var element = TableMapdat.LoadElementFromSrc(src);
            if (!element) {
                return;
            }
            _grid = element.gird;
            foreach (var item in element.items) {
                var data = new _Data();
                data.index = item.index;
                data.id = item.id;
                data.position = item.position;
                data.hpMax = item.hpMax;
                data.handleObject = BrickFactory.Create(data.id, data.hpMax, data.position, null)?.gameObject;
                _dataMap.Add(data.index, data);
            }
        }

        private void _SaveToFile(string filename) {
            _ClearOutOfContent();

            if (_dataMap.Count == 0) {
                return;
            }
            bool bExist = false;

            string ret = "{";
            ret += "\"grid\":{\"x\":";
            ret += _grid.x;
            ret += ",\"y\":";
            ret += _grid.y;
            ret += "},\"data\":[";
            foreach (var item in _dataMap) {
                var value = item.Value;
                if (value.handleObject) {
                    ret += "[";
                    ret += value.index;
                    ret += ",";
                    ret += value.id;
                    ret += ",";
                    ret += value.position.x;
                    ret += ",";
                    ret += value.position.y;
                    ret += ",";
                    ret += value.hpMax;
                    ret += "],";

                    bExist = true;
                }
            }
            if (!bExist) {
                return;
            }
            ret = ret.Remove(ret.Length - 1);
            ret += "]}";

            if (!Directory.Exists("user")) {
                Directory.CreateDirectory("user");
            }
            if (!Directory.Exists(DIRECTORY_MAPDATA)) {
                Directory.CreateDirectory(DIRECTORY_MAPDATA);
            }
            var srcFilename = DIRECTORY_MAPDATA + filename + ".json";
            File.WriteAllText(srcFilename, ret);
            File.Copy(srcFilename, ASSET_MAPDATA + filename + ".json", true);
            Debug.Log("saved!");
        }

#if UNITY_EDITOR
        private const string DIRECTORY_MAPDATA = "user/mapdata/";
        private const string ASSET_MAPDATA = "Assets/Res/mapdata/";
#else
        private const string DIRECTORY_MAPDATA = "../user/mapdata/";
        private const string ASSET_MAPDATA = "../Assets/Res/mapdata/";
#endif
    }
}