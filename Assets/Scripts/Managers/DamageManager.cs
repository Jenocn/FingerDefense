using GCL.Pattern;
using UnityEngine;

namespace Game.Managers {
	public class DamageResult {
		public int resultHP = 0;
		public int damageValue = 0;
		public bool bCrit = false;
	}
	public class DamageManager : ManagerBase<DamageManager> {
		public DamageResult CalcDamage(int curHP, AttackData attackData) {
			if (GCL.Base.RandomTool.HitWithPercent(attackData.critPercent)) {
				return CalcDamageCrit(curHP, attackData);
			}
			return CalcDamageNormal(curHP, attackData);
		}
		public DamageResult CalcDamageNormal(int curHP, AttackData attackData) {
			var ret = new DamageResult();
			ret.resultHP = Mathf.Max(0, curHP - attackData.attack);
			ret.damageValue = curHP - ret.resultHP;
			ret.bCrit = false;
			return ret;
		}
		public DamageResult CalcDamageCrit(int curHP, AttackData attackData) {
			var ret = new DamageResult();
			ret.resultHP = Mathf.Max(0, Mathf.FloorToInt(curHP - attackData.attack * (1 + attackData.critAttackPercentAdd)));
			ret.damageValue = curHP - ret.resultHP;
			ret.bCrit = true;
			return ret;
		}
	}
}