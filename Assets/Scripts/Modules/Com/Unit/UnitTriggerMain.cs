using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {

    [RequireComponent(typeof(UnitID))]
    public class UnitTriggerMain : MonoBehaviour {
        private UnitID _unitID = null;
        public SimpleNotify<Collider2D> triggerNotify { get; private set; } = new SimpleNotify<Collider2D>();

        private void Awake() {
            _unitID = GetComponent<UnitID>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!enabled) { return; }
            var otherID = other.GetComponent<UnitID>();
            if (!otherID) { return; }
            if (otherID.elementType == _unitID.elementType) { return; }
            var otherTriggerSub = other.GetComponent<UnitTriggerSub>();
            if (otherTriggerSub) {
                if (otherTriggerSub.enabled) {
                    otherTriggerSub.OnTriggerSub(GetComponent<Collider2D>());
                    triggerNotify.Send(other);
                }
            } else {
                triggerNotify.Send(other);
            }
        }
    }
}