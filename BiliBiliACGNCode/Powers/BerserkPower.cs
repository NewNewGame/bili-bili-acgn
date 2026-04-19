//****************** 代码文件申明 ***********************
//* 文件：RagePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 红怒
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Nodes;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BerserkPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsInstanced => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageMultiplier", 150m), new EnergyVar(2), new CardsVar(2)];
    /// <summary>
    /// 应用前
    /// </summary>
    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果目标为空，则返回
        if(target == null) return Task.CompletedTask;
        // 如果目标已经有红怒，则返回
        if(target.HasPower<BerserkPower>())
        {
            return Task.CompletedTask;
        }
        // 添加红怒VFX
        var vfx = CustomVfxCmd.AddVfxOnCenter(target, CustomVfxCmd.BerserkPath);
        // 将VFX放在最后面
        if(vfx != null)
        {
            vfx.GetParent().MoveChild(vfx, 0);
        }
        return Task.CompletedTask;
    }
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果施加者不是玩家，则返回
        if(base.Owner.Player == null) return;
        // 如果不是红怒，则返回
        if(power != this || amount < 0 || power.Owner != base.Owner) return;
        
        // 回复能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner.Player);
        // 如果小于手牌上限，则抽牌
        if(PileType.Hand.GetPile(base.Owner.Player).Cards.Count < CombatUtils.HandMaxCount){
            await CardPileCmd.Draw(CombatUtils.GetTemporaryPlayerChoiceContext(), base.DynamicVars.Cards.BaseValue, base.Owner.Player);
        }
    }
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
		if (dealer != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (target == null)
		{
			return 1m;
		}
        var damageMultiplier = base.DynamicVars["DamageMultiplier"].BaseValue / 100m;
        if(amount * damageMultiplier >= int.MaxValue)
        {
            return 1m;
        }
        return damageMultiplier;
    }
    // 回合结束后失去红怒
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }
    public override async Task AfterRemoved(Creature oldOwner)
    {
        // 如果旧所有者不是当前所有者，则返回
        if(oldOwner != base.Owner) return;
        // 如果有开始享受能力，并且是玩家
        int num = oldOwner.GetPowerAmount<EnjoyPower>();
        if(num > 0 && oldOwner.Player != null){
            // 移除后抽牌
            await CardPileCmd.Draw(CombatUtils.GetTemporaryPlayerChoiceContext(), base.Amount, oldOwner.Player);
        }
        // 如果旧所有者没有红怒，则移除红怒VFX
        if(!oldOwner.HasPower<BerserkPower>())
        {
            CustomVfxCmd.RemoveVfx<SNBerserkVfx>(oldOwner);
        }
    }
    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 如果旧所有者不是当前所有者，则返回
        if(creature != base.Owner) return Task.CompletedTask;
        // 移除红怒VFX
        CustomVfxCmd.RemoveVfx<SNBerserkVfx>(creature);

        return Task.CompletedTask;
    }
    public override Task AfterPreventingDeath(Creature creature)
    {
        // 如果旧所有者不是当前所有者，则返回
        if(creature != base.Owner) return Task.CompletedTask;
        // 如果旧所有者有红怒，则添加红怒VFX
        if(creature.HasPower<BerserkPower>())
        {
            CustomVfxCmd.AddVfxOnCenter(creature, CustomVfxCmd.BerserkPath);
        }
        return Task.CompletedTask;
    }


}