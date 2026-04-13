//****************** 代码文件申明 ***********************
//* 文件：DelayedStrengthPower(延迟力量)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：在你下一个回合开始时，获得力量（数值见 Amount / CanonicalVars）。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class DelayedStrengthPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // TODO: 仅在「下一」玩家回合开始时生效：对 base.Owner 施加 StrengthPower（层数取 Strength/Amount）；随后移除本能力
        return Task.CompletedTask;
    }
}
