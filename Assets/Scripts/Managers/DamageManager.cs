using GCL.Pattern;
using UnityEngine;

namespace Game.Managers {
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