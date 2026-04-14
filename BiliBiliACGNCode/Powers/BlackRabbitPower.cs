//****************** 代码文件申明 ***********************
//* 文件：BlackRabbitPower(黑兔BUFF)
//* 作者：wheat
//* 创建时间：2026/04/14
//* 描述：每当你激发3个充能球，获得1个充能球栏位。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

//****************** 代码文件申明 ***********************
//* 文件：BlackRabbitPower(黑兔BUFF)
//* 作者：wheat
//* 创建时间：2026/04/14
//* 描述：每当你激发3个充能球，获得1个充能球栏位。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BlackRabbitPower : PowerBaseModel
{
    private class Data{
        public int EvokeCount;
    }
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    public override int DisplayAmount => base.Amount - GetInternalData<Data>().EvokeCount;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];
    protected override object? InitInternalData()
    {
        return new Data();
    }
    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if(orb.Owner != base.Owner.Player){
            return;
        }
        GetInternalData<Data>().EvokeCount++;
        if(GetInternalData<Data>().EvokeCount >= base.Amount){
            GetInternalData<Data>().EvokeCount = 0;
            await OrbCmd.AddSlots(base.Owner.Player, 1);
        }
        InvokeDisplayAmountChanged();
    }
}