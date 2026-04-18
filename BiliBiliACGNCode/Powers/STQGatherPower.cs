//****************** 代码文件申明 ***********************
//* 文件：STQGatherPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 7tq集合
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class STQGatherPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 进入红怒时所有盟友获得 Amount 点力量
        if(power.Owner == base.Owner && power is BerserkPower && amount > 0){
            // 获取所有盟友
            var allies = base.CombatState.Creatures.Where(c => c != base.Owner && c.Side == base.Owner.Side && c.IsAlive).ToList();
            foreach(var ally in allies){
                await PowerCmd.Apply<StrengthPower>(ally, base.Amount, base.Owner, null);
            }
        }
    }

}
