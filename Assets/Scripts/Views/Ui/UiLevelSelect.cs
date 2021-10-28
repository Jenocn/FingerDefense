using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Game.Systems;
using Game.Tables;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.UI;
using UnityUiModel;

namespace Game.Views {
    public class UiLevelSelect : UiModel {
        public class Item {
            public int id = 0;
            public RectTransform transform = null;
            public Button button = null;
            public Text textIndex = null;
            public Text textRank = null;
        }

        private MapManager _mapManager = null;
        private ScoreManager _scoreManager = null;
        private List<int> _normalIDs = null;
        private int _currentPage = -1;
        private int _maxPage = 0;
        private List<Item> _items = new List<Item>();
        private int _row = 4;
        private int _line = 5;
        private readonly string[] _starList = new string[] { "☆☆☆", "★☆☆", "★★☆", "★★★" };
        public override void OnInitUI() {
            _mapManager = ManagerCenter.GetManager<MapManager>();
            _scoreManager = ManagerCenter.GetManager<ScoreManager>();
            _normalIDs = TableMapdat.instance.GetNormalIDs();

            _maxPage = (_normalIDs.Count - 1) / _row / _line;

            var prefab = AssetSystem.Load<GameObject>("prefabs", "UiLevelSelect");
            var ui = InstantiateUI(prefab).transform;

            var itemPrefab = AssetSystem.Load<GameObject>("prefabs", "UiLevelSelect_Button");
            var root = ui.Find("Root");

            for (var i = 0; i < _row * _line; ++i) {
                var x = i % _row;
                var y = _line - i / _row - 1;

                var item = new Item();
                item.transform = Instantiate<GameObject>(itemPrefab, root).GetComponent<RectTransform>();
                var size = item.transform.sizeDelta + new Vector2(8, 8);
                item.transform.localPosition = new Vector3(size.x * (x + 0.5f - _row / 2), size.y * (y - Mathf.FloorToInt(_line / 2)), 0);

                item.button = item.transform.GetComponent<Button>();
                var index = i;
                item.button.onClick.AddListener(() => {
                    _OnSelect(item);
                });
                item.textIndex = item.transform.Find("TextIndex").GetComponent<Text>();
                item.textIndex.text = i.ToString();
                item.textRank = item.transform.Find("TextRank").GetComponent<Text>();
                _items.Add(item);
            }

            ui.Find("ButtonBack").GetComponent<Button>().onClick.AddListener(() => {
                MessageCenter.Send(new UiMessage_OnButtonGameBack());
            });
            ui.Find("ButtonUp").GetComponent<Button>().onClick.AddListener(() => {
                _ShowPrevPage();
            });
            ui.Find("ButtonDown").GetComponent<Button>().onClick.AddListener(() => {
                _ShowNextPage();
            });
            _ShowPage(0);
        }

        private void _OnSelect(Item item) {
            MessageCenter.Send(new UiMessage_OnClassicLevelSelect(item.id));
        }
        private void _ShowPage(int page) {
            page = Mathf.Clamp(page, 0, _maxPage);
            if (_currentPage == page) {
                return;
            }

            var index0 = page * _items.Count;
            for (var i = 0; i < _items.Count; ++i) {
                var item = _items[i];
                item.textIndex.text = (index0 + 1).ToString();

                if (index0 < _normalIDs.Count) {
                    var mapID = _normalIDs[index0];
                    item.id = mapID;
                    item.transform.gameObject.SetActive(true);
                    var star = Mathf.Clamp(_scoreManager.GetClassicStar(mapID), 0, 3);
                    item.textRank.text = _starList[star];
                } else {
                    item.transform.gameObject.SetActive(false);
                }
                ++index0;
            }
        }
        private void _ShowNextPage() {
            var page = _currentPage + 1;
            if (page > _maxPage) {
                page = 0;
            }
            _ShowPage(page);
        }
        private void _ShowPrevPage() {
            var page = _currentPage - 1;
            if (page < 0) {
                page = _maxPage;
            }
            _ShowPage(page);
        }
    }
}