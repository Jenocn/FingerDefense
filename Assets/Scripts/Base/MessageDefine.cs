using GCL.Pattern;
using UnityEngine;

/// <summary>
/// 游戏暂停/继续的消息
/// </summary>
public class MessageGamePause : MessageBase<MessageGamePause> {
	public MessageGamePause(bool bPause) {
		this.bPause = bPause;
	}
	public bool bPause { get; private set; } = false;
}

/// <summary>
/// 砖块被命中消息
/// </summary>
public class MessageBrickHit : MessageBase<MessageBrickHit> {
	public MessageBrickHit(int uniqueID, DamageResult result, Vector3 position, int attackID, ID_ElementType attackElementType) {
		this.uniqueID = uniqueID;
		this.damageResult = result;
		this.position = position;
		this.attackID = attackID;
		this.attackElementType = attackElementType;
	}
	public int uniqueID { get; private set; } = 0;
	public DamageResult damageResult { get; private set; } = null;
	public Vector3 position { get; private set; } = Vector3.zero; // 砖块坐标
	public int attackID { get; private set; } = 0;
	public ID_ElementType attackElementType { get; private set; } = 0;
}