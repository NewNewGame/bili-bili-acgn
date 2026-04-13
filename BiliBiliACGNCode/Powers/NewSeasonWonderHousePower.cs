//****************** 代码文件申明 ***********************
//* 文件：NewSeasonWonderHousePower(新番妙妙屋)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你打出能力牌时，获得 Amount 点能量
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonWonderHousePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每当你打出能力牌时，获得 Amount 点能量
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner != base.Owner.Player) return;
        if(cardPlay.Card.Type != CardType.Power) return;
        await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
    }
}
