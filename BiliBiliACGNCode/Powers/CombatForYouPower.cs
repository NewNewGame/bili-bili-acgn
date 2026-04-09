//****************** 代码文件申明 ***********************
//* 文件：CombatForYouPower
//* 作者：wheat
//* 创建时间：2026/04/09
//* 描述：为你而战 女儿会[gold]保护[/gold]你，帮会吸收所有未被格挡的攻击伤害，为你而战，但不会为了你去死，过量伤害返还。
//*******************************************************


using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;
public sealed class CombatForYouPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;

	/// <summary>
	/// 未被格挡时，女儿吸收过量伤害
	/// </summary>
	public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
	{
		if (target != base.Owner.PetOwner?.Creature)
		{
			return target;
		}
		if (base.Owner.IsDead)
		{
			return target;
		}
		if (!props.IsPoweredAttack_())
		{
			return target;
		}
		return base.Owner;
	}
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
	public override async Task AfterPreventingDeath(Creature creature)
	{
		if(creature != base.Owner) return;
		await CreatureCmd.Heal(creature, 1, false);
		// 返还过量伤害
		if(creature.PetOwner != null){
			await CreatureCmd.Damage(CombatUtils.GetTemporaryPlayerChoiceContext(), creature.PetOwner.Creature, 1, ValueProp.Unpowered, null, null);
		}
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
	/// <summary>
	/// 获得格挡时，给予临时提升生命值
	/// </summary>
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
		if(creature != base.Owner) return;
        await PowerCmd.Apply<AddMaxHpTempPower>(base.Owner, amount, base.Owner, null);
    }



}