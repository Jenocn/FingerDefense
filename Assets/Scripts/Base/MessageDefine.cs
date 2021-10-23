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
/// 球碰撞消息
/// </summary>
public class MessageBallCollision : MessageBase<MessageBallCollision> {
	public MessageBallCollision(int ballID, Vector3 ballPosition, Vector3 collisionPosition) {
		this.ballID = ballID;
		this.ballPosition = ballPosition;
		this.collisionPosition = collisionPosition;
	}
	public int ballID { get; private set; } = 0;
	public Vector3 ballPosition { get; private set; } = Vector3.zero;
	public Vector3 collisionPosition { get; private set; } = Vector3.zero;
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

/// <summary>
/// 撞击到球拍消息
/// </summary>
public class MessageRacketHit : MessageBase<MessageRacketHit> {
	public MessageRacketHit(int racketID, Vector3 position, int ballID) {
		this.racketID = racketID;
		this.position = position;
		this.ballID = ballID;
	}

	public int racketID { get; private set; } = 0;
	public Vector3 position { get; private set; } = Vector3.zero; // 球拍坐标
	public int ballID { get; private set; } = 0;
}

/// <summary>
/// 球掉入陷阱消息
/// </summary>
public class MessageFallIntoTrap : MessageBase<MessageFallIntoTrap> {
	public MessageFallIntoTrap(int ballID) {
		this.ballID = ballID;
	}
	public int ballID { get; private set; } = 0;
}

/// <summary>
/// 分数变化时
/// </summary>
public class MessageScoreChange : MessageBase<MessageScoreChange> {
	public MessageScoreChange(int score, int previous, int delta) {
		this.score = score;
		this.previous = previous;
		this.delta = delta;
	}
	public int score { get; private set; } = 0;
	public int previous { get; private set; } = 0;
	public int delta { get; private set; } = 0;
}

/// <summary>
/// 游戏失败
/// </summary>
public class MessageGameFailed : MessageBase<MessageGameFailed> {}

/// <summary>
/// 播放背景音乐消息
/// </summary>
public class MessageMusicPlay : MessageBase<MessageMusicPlay> {
	public MessageMusicPlay(MusicChannelType channelType, string audioName, float volume, bool bLoop) {
		this.channelType = channelType;
		this.audioName = audioName;
		this.volumePercent = volume;
		this.bLoop = bLoop;
	}
	public MusicChannelType channelType { get; private set; } = MusicChannelType.BGM;
	public string audioName { get; private set; } = "";
	public float volumePercent { get; private set; } = 1;
	public bool bLoop { get; private set; } = true;
}
/// <summary>
/// 背景音乐控制消息
/// </summary>
public class MessageMusicControl : MessageBase<MessageMusicControl> {
	public MessageMusicControl(MusicChannelType channelType, AudioControlType controlType) {
		this.channelType = channelType;
		this.controlType = controlType;
	}
	public MusicChannelType channelType { get; private set; } = MusicChannelType.BGM;
	public AudioControlType controlType { get; private set; } = AudioControlType.Play;
}
/// <summary>
/// 播放音效消息
/// </summary>
public class MessageSoundEffect : MessageBase<MessageSoundEffect> {
	public MessageSoundEffect(string audioName, float volume) {
		this.audioName = audioName;
		this.volumePercent = volume;
	}
	public string audioName { get; private set; } = "";
	public float volumePercent { get; private set; } = 1;
}