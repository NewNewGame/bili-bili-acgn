//****************** 代码文件申明 ***********************
//* 文件：LoveBrainPower(恋爱脑)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你给予 Amount 次病态后，获得 1 点能量（内部计数 TODO）
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class LoveBrainPower : PowerBaseModel
{
    private class Data
    {
        public int appliedTime;
    }
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	public override bool IsInstanced => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    public override int DisplayAmount => base.Amount - GetInternalData<Data>().appliedTime;
    protected override object InitInternalData()
    {
        return new Data();
    }
    // 每当你给予 Amount 次病态后，获得 1 点能量
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(applier == null || base.Owner.Player == null) return;
        if(amount > 0 && power is MorbidPower && (applier == base.Owner || applier.PetOwner == base.Owner.Player)){
            var data = GetInternalData<Data>();
            data.appliedTime++;
            if(data.appliedTime >= base.Amount){
                await PlayerCmd.GainEnergy(1, base.Owner.Player);
                data.appliedTime = 0;
            }
        }
    }
}
