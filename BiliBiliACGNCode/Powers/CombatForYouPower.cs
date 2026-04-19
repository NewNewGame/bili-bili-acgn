//****************** 代码文件申明 ***********************
//* 文件：CombatForYouPower
//* 作者：wheat
//* 创建时间：2026/04/09
//* 描述：为你而战 女儿会[gold]保护[/gold]你，帮会吸收所有未被格挡的攻击伤害，为你而战，但不会为了你去死，过量伤害返还。
//*******************************************************


using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;
public sealed class CombatForYouPower : PowerBaseModel
{
	private class Data
	{
		public bool returnDamage;
	}
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;
	protected override object InitInternalData()
	{
		return new Data();
	}

	/// <summary>
	/// 未被格挡时，女儿吸收过量伤害
	/// </summary>
	public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
	{
		if (target != base.Owner.PetOwner?.Creature)
		{
			return target;
		}
		if (base.Owner.IsDead || base.Owner.CurrentHp == 1)
		{
			return target;
		}
		if (!props.IsPoweredAttack_())
		{
			return target;
		}
		return base.Owner;
	}
	#region 死亡相关
	/// <summary>
	/// 女儿不会为了你去死`
	/// </summary>
    public override bool ShouldDieLate(Creature creature)
    {
        if(creature != base.Owner) return true;
        return false;
    }
	/// <summary>
	/// 死亡后马上复活
	/// 返还过量伤害
	/// </summary>
	public override Task AfterPreventingDeath(Creature creature)
	{
		if(creature != base.Owner) return Task.CompletedTask;
		// 复活
		creature.HealInternal(1);
		return Task.CompletedTask;
	}
	/// <summary>
	/// 死亡后BUFF不移除
	/// </summary>
	/// <returns></returns>
	public override bool ShouldPowerBeRemovedAfterOwnerDeath()
	{
		return false;
	}
	public override bool ShouldAllowHitting(Creature creature)
	{
		return creature.IsAlive;
	}
	public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
	{
		if (creature != base.Owner)
		{
			return true;
		}
		return false;
	}
	#endregion
	/// <summary>
	/// 获得格挡时，给予临时提升生命值
	/// </summary>
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
		if(creature != base.Owner) return;
        await PowerCmd.Apply<AddMaxHpTempPower>(base.Owner, amount, base.Owner, null);
    }
	/// <summary>
	/// 修改伤害计算
	/// </summary>
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
		// 如果目标是玩家
		if(target == base.Owner.PetOwner?.Creature){
			// 如果伤害返还，则返回原始伤害+1
			var data = base.GetInternalData<Data>();
			if(data.returnDamage){
				data.returnDamage = false;
				return amount + 1;
			}
			// 如果伤害未返还，则返回原始伤害
			return amount;
		}
		// 如果目标不是自己，则返回原始伤害
        if (target != base.Owner)
		{
			return amount;
		}
		// 如果施加者为空，则返回原始伤害
		if (dealer == null)
		{
			return amount;
		}
		// 如果可以击败女儿，那就伤害+1返还给玩家。
		if (amount >= base.Owner.CurrentHp)
		{
			// 设置伤害返还
			var data = base.GetInternalData<Data>();
			data.returnDamage = true;
			return amount;
		}
        return amount;
    }

}