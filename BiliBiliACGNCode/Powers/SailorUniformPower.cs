//****************** 代码文件申明 ***********************
//* 文件：SailorUniformPower(水手服)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当女儿获得格挡时，对随机敌人造成 Amount 点伤害
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SailorUniformPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 每当女儿获得格挡时，对随机敌人造成 Amount 点伤害。
    /// </summary>
    /// <param name="creature"></param>
    /// <param name="amount"></param>
    /// <param name="props"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(amount <= 0 || power is not AddMaxHpTempPower) return;
        if(applier != base.Owner) return;
        if(base.CombatState == null) return;
        // 随机获得一个敌人
        var enemy = base.CombatState.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
        if(enemy == null) return;
        await CreatureCmd.Damage(CombatUtils.GetTemporaryPlayerChoiceContext(), enemy, base.Amount, ValueProp.Unpowered,base.Owner);
    }
}
