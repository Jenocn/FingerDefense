using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using Game.Tables;
using GCL.Serialization;
using UnityEngine;

namespace Game.Managers {
    public class MapManager : ManagerBase<MapManager> {

        public int currentID { get; private set; } = 1;
        public MapMode mapMode { get; private set; } = MapMode.Classic;
        public int classicPage { get; private set; } = 0;
        private List<int> _normalIDs = null;
        private Dictionary<int, bool> _mapLockDict = new Dictionary<int, bool>();

        public override void OnInitManager() {
            _normalIDs = TableMapdat.instance.GetNormalIDs();
            _mapLockDict.Clear();
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

        public bool IsClassicLocked(int mapID) {
            if (_mapLockDict.TryGetValue(mapID, out var v)) {
                return v;
            }
            return true;
        }

        public void SetClassicLocked(int mapID, bool bLocked) {
            if (_mapLockDict.ContainsKey(mapID)) {
                _mapLockDict[mapID] = bLocked;
            } else {
                _mapLockDict.Add(mapID, bLocked);
            }
        }

        public void SetNextMapClassicLocked(int currentMapID, bool bLocked) {
            var index = GetClassicIndex(currentMapID);
            if (index >= 0 && index < _normalIDs.Count - 1) {
                var nextID = _normalIDs[index + 1];
                SetClassicLocked(nextID, bLocked);
            }
        }

        public override void OnArchiveLoaded(ArchiveSystem.Archive archive) {
            var src = archive.GetString("MapManager", "MapLock", "");
            _mapLockDict = JSONTool.ParseToCustomKV<int, bool>(src);

            if (_normalIDs.Count > 0) {
                SetClassicLocked(_normalIDs[0], false);
            }
        }

        public override void OnArchiveSaveBegin(ArchiveSystem.Archive archive) {
            archive.SetString("MapManager", "MapLock", JSONTool.ToString(_mapLockDict));
        }
    }
}