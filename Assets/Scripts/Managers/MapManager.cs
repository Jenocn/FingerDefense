using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers {
    public class MapManager : ManagerBase<MapManager> {

        public int currentID { get; private set; } = 1;
        public MapMode mapMode { get; private set; } = MapMode.Classic;

        public void SetCurrent(MapMode mode, int id) {
            mapMode = mode;
            currentID = id;
        }
    }
}