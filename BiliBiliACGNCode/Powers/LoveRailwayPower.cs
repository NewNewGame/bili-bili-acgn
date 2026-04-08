//****************** 代码文件申明 ***********************
//* 文件：LoveRailwayPower(爱洗铁路)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每打出1张能力牌，给予随机敌人病态。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class LoveRailwayPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    /// <summary>
    /// Amount：每次触发给予的病态层数
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="cardPlay"></param>
    /// <returns></returns>
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card.Type == CardType.Power)
        {
            await PowerCmd.Apply<MorbidPower>(base.Owner, Amount, base.Owner, null);
        }
    }
}
