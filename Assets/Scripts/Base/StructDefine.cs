
[System.Serializable]
public class HealthData {
    // 生命最大值
    public int hpMax = 0;
}

[System.Serializable]
public class AttackData {
    // 攻击力
    public int attack = 0;
    // 暴击几率百分比
    public float critPercent = 0;
    // 暴击攻击百分比加成
    public float critAttackPercentAdd = 1;
}