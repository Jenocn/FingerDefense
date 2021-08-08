using GCL.Pattern;
using UnityEngine;

/// <summary>
/// 游戏暂停/继续的消息
/// </summary>
public class MessageGamePause : MessageBase<MessageGamePause> {
	public bool bPause = false;
}
