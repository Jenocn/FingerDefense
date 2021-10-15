using GCL.Pattern;
using UnityEngine;

public class PeakMessage_CreateEffect : MessageBase<PeakMessage_CreateEffect> {
	public int effectID = 0;
	public Vector2 position = Vector2.zero;
	public float delay = 0;
}