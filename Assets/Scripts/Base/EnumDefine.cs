public enum DestroyType {
    None = 0,
    Death,
}

public enum ID_ElementType {
    None = 0,
    Ball, // 球
    Brick, // 砖块
    Racket, // 球拍
    Edge, // 边缘
    Bomb, // 炸弹
    Trap, // 陷阱
}

public enum ID_CampType {
    Player, // 我方
    Enemy, // 敌方
}

public enum AudioType {
    Music = 0,
    Effect,
    Voice,
}

public enum MixerType {
    Main,
    Music,
    Effect,
    Voice,
}
public enum AudioControlType {
    None = 0,
    Play,
    Pause,
    UnPause,
    Replay,
    Stop,
}

public enum MusicChannelType {
    BGM = 0,
    Side1,
    Side2,
    Side3,
}

public enum MapMode {
    None = 0,
    Classic, // 经典模式
    Infinite, // 无限模式
    Challenge, // 挑战模式
}
