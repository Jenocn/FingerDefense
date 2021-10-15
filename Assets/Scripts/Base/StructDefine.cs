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

public class DamageResult {
    // 返回的HP
    public int resultHP { get; private set; } = 0;
    // 本次伤害的数值
    public int damageValue { get; private set; } = 0;
    // 是否暴击
    public bool bCrit { get; private set; } = false;
    // 是否死亡
    public bool bDie { get; private set; } = false;

    public DamageResult(int resultHP, int damageValue, bool bCrit) {
        this.resultHP = resultHP;
        this.damageValue = damageValue;
        this.bCrit = bCrit;
        bDie = (resultHP <= 0);
    }
}