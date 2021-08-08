using UnityEngine;

namespace Game.Modules {
    public class UnitID : MonoBehaviour {
        [SerializeField]
        private int _uniqueID = 0;
        public int uniqueID { get => _uniqueID; }

        [SerializeField]
        private ElementType _elementType = ElementType.None;
        public ElementType elementType { get => _elementType; }

    }
}