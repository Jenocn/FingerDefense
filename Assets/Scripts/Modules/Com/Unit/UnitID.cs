using UnityEngine;

namespace Game.Modules {
    public class UnitID : MonoBehaviour {
        [SerializeField]
        private int _uniqueID = 0;
        public int uniqueID { get => _uniqueID; }

        [SerializeField]
        private ID_ElementType _elementType = ID_ElementType.None;
        public ID_ElementType elementType { get => _elementType; }

        [SerializeField]
        private ID_CampType _campType = ID_CampType.Enemy;
        public ID_CampType campType { get => _campType; }
    }
}