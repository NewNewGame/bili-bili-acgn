//****************** 代码文件申明 ***********************
//* 文件：DelayOrbPower
//* 作者：wheat
//* 创建时间：2026/04/14 10:20:49 星期二
//* 描述：延迟充能球 在你下一个回合开始时，获得1个随机充能球。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class DelayOrbPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];
    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if(player != base.Owner.Player){
            return;
        }
        int amount = base.Amount;
        await PowerCmd.Remove(this);
        while(amount > 0){
            await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(base.Owner.CombatState), base.Owner.Player);
            amount--;
        }
    }
}