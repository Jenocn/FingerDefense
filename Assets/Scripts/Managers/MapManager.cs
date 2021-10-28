using System.Collections;
using System.Collections.Generic;
using Game.Tables;
using UnityEngine;

namespace Game.Managers {
    public class MapManager : ManagerBase<MapManager> {

        public int currentID { get; private set; } = 1;
        public MapMode mapMode { get; private set; } = MapMode.Classic;
        public int classicPage { get; private set; } = 0;
        private List<int> _normalIDs = null;

        public override void OnInitManager() {
            _normalIDs = TableMapdat.instance.GetNormalIDs();
        }

        public void SetCurrent(MapMode mode, int id) {
            mapMode = mode;
            currentID = id;
        }

        public void SetClassicPage(int page) {
            classicPage = page;
        }

        public void SetCurrentNext() {
            if (mapMode == MapMode.Classic) {
                var index = GetClassicIndex(currentID);
                if (index + 1 < _normalIDs.Count) {
                    SetCurrent(mapMode, _normalIDs[index + 1]);
                }
            }
        }

        public bool HasClassicNext() {
            return GetClassicIndex(currentID) + 1 < _normalIDs.Count;
        }

        public int GetClassicIndex(int id) {
            return _normalIDs.FindIndex((int i) => {
                return i == id;
            });
        }
    }
}