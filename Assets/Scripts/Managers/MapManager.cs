using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers {
    public class MapManager : ManagerBase<MapManager> {
        private int _currentID = 0;
        public int currentID { get => _currentID; }

        public void SetCurrentID(int id) {
            _currentID = id;
        }
    }
}