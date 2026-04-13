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
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonWonderHousePower : PowerBaseModel
{
    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
    }
    public override PowerType Type => PowerType.Buff;
    protected override object InitInternalData()
    {
        return new Data();
    }
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner != base.Owner.Player) return Task.CompletedTask;
        if(cardPlay.Card.Type != CardType.Power) return Task.CompletedTask;
        GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, base.Amount);
        return Task.CompletedTask;
    }
    // 每当你打出能力牌时，获得 Amount 点能量
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player && GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out var value) && value > 0)
        {
            await PlayerCmd.GainEnergy(value, base.Owner.Player);
        }
    }
}
