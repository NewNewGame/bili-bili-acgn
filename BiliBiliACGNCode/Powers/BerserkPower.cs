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
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using BiliBiliACGN.BiliBiliACGNCode.Utils;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BerserkPower : PowerBaseModel
{
    protected override string customIconPath => "berserk";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsInstanced => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageMultiplier", 50m), new EnergyVar(3), new CardsVar(2)];

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果施加者不是玩家，则返回
        if(base.Owner.Player != null && base.Owner != applier) return;
        // 如果不是红怒，则返回
        if(power is not BerserkPower || amount < 0 || power != this) return;
        
        // 回复能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner.Player);
        await CardPileCmd.Draw(CombatHelper.GetTemporaryPlayerChoiceContext(), base.DynamicVars.Cards.BaseValue, base.Owner.Player);
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

        return 1m + base.DynamicVars["DamageMultiplier"].BaseValue/100m;
    }
    // 回合结束后失去红怒
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.Remove(this);
        }
    }
}