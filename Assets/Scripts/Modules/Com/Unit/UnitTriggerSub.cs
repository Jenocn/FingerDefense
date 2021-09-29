using System.Collections;
using System.Collections.Generic;
using GCL.Pattern;
using UnityEngine;

namespace Game.Modules {
    public class UnitTriggerSub : MonoBehaviour {
        public SimpleNotify<Collider2D> triggerNotify { get; private set; } = new SimpleNotify<Collider2D>();
        public void OnTriggerSub(Collider2D other) {
            triggerNotify.Send(other);
        }
    }
}