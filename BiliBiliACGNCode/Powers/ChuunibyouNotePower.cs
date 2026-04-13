//****************** 代码文件申明 ***********************
//* 文件：ChuunibyouNotePower(中二笔记)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当你打出能力牌时，抽牌。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ChuunibyouNotePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Amount：每次触发抽牌数量
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="cardPlay"></param>
    /// <returns></returns>
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card.Type == CardType.Power)
        {
            await CardPileCmd.Draw(choiceContext, Amount, base.Owner.Player);
        }
    }
}
