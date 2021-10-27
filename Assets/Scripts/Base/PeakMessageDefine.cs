using GCL.Pattern;
using UnityEngine;

public class PeakMessage_CreateEffect : MessageBase<PeakMessage_CreateEffect> {
	public int effectID = 0;
	public Vector2 position = Vector2.zero;
	public float delay = 0;
}
public class PeakMessage_CreateBall : MessageBase<PeakMessage_CreateBall> {
	public int ballID = 0;
	public Vector2 position = Vector2.zero;
	public Vector2 direction = Vector2.one;
	public float delay = 0;
}
public class PeakMessage_CreateBrick : MessageBase<PeakMessage_CreateBrick> {
	public int brickID = 0;
	public Vector2 position = Vector2.zero;
	public int hpMax = 0;
}
public class PeakMessage_CreateInfiniteBrick : MessageBase<PeakMessage_CreateInfiniteBrick> {
	public int index = 0;
	public int brickID = 0;
	public Vector2 position = Vector2.zero;
	public int hpMax = 0;
}