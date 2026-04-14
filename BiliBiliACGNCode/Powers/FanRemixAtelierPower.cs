//****************** 代码文件申明 ***********************
//* 文件：FanRemixAtelierPower(泛式二创屋)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：在接下来若干回合开始时，各生成1个进攻充能球（剩余次数为 Amount）。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class FanRemixAtelierPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<AttackOrb>(),
        HoverTipFactory.Static(CustomeHoverTips.AttackOrb)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player != base.Owner.Player)
        {
            return;
        }
        await OrbCmd.Channel<AttackOrb>(choiceContext, base.Owner.Player);
        await PowerCmd.Decrement(this);
    }
}
