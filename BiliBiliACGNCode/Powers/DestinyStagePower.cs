//****************** 代码文件申明 ***********************
//* 文件：DestinyStagePower(命运舞台)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你打出能力牌时，随机生成 Amount 个充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class DestinyStagePower : PowerBaseModel
{
    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
    }
    protected override object InitInternalData()
    {
        return new Data();
    }
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner != base.Owner.Player) return Task.CompletedTask;
        if(cardPlay.Card.Type != CardType.Power) return Task.CompletedTask;
        GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, base.Amount);
        return Task.CompletedTask;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];
    /// <summary>
    /// 每当你打出能力牌时，随机生成 Amount 个充能球
    /// </summary>
    /// <param name="choiceContext">选择上下文</param>
    /// <param name="cardPlay">卡牌打出</param>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player && GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out var value) && value > 0)
        {
            for(int i = 0; i < value; i++){
                await OrbCmd.Channel(context, OrbUtils.GetRandomFunShikiOrb(cardPlay.Card), base.Owner.Player);
                if(i < value - 1)
                {
                    await Cmd.Wait(0.1f);
                }
            }
        }
    }

}
