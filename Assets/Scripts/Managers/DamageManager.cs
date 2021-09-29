using GCL.Pattern;
using UnityEngine;

namespace Game.Managers {
	public class DamageResult {
		public int resultHP { get; private set; } = 0;
		public int damageValue { get; private set; } = 0;
		public bool bCrit { get; private set; } = false;
		public bool bDie { get; private set; } = false;

		public DamageResult(int resultHP, int damageValue, bool bCrit) {
			this.resultHP = resultHP;
			this.damageValue = damageValue;
			this.bCrit = bCrit;
			bDie = (resultHP <= 0);
		}
	}
	public class DamageManager : ManagerBase<DamageManager> {
		public DamageResult CalcDamage(int curHP, AttackData attackData) {
			if (GCL.Base.RandomTool.HitWithPercent(attackData.critPercent)) {
				return CalcDamageCrit(curHP, attackData);
			}
			return CalcDamageNormal(curHP, attackData);
		}
		public DamageResult CalcDamageNormal(int curHP, AttackData attackData) {
			var resultHP = Mathf.Max(0, curHP - attackData.attack);
			var damageValue = curHP - resultHP;
			return new DamageResult(resultHP, damageValue, false);
		}
		public DamageResult CalcDamageCrit(int curHP, AttackData attackData) {
			var resultHP = Mathf.Max(0, Mathf.FloorToInt(
				curHP - attackData.attack * (1 + attackData.critAttackPercentAdd)));
			var damageValue = curHP - resultHP;
			return new DamageResult(resultHP, damageValue, true);
		}
	}
}