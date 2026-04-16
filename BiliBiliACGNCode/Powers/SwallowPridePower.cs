//****************** 代码文件申明 ***********************
//* 文件：SwallowPridePower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 忍气吞声
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SwallowPridePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    //进入红怒时获得能量与力量
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power.Owner == base.Owner && power is BerserkPower && amount > 0){
            Flash();
            await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
            await PowerCmd.Apply<StrengthPower>(base.Owner, base.Amount, base.Owner, null);
            await PowerCmd.Remove<SwallowPridePower>(base.Owner);
        }
    }
}
