using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEdge : MonoBehaviour {
    [SerializeField]
    private Vector2 _direction = Vector2.zero;
    public Vector2 direction { get => _direction; }
}