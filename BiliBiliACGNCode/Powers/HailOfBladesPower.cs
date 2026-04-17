//* 文件：HailOfBladesPower
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：能力 丛刃 本回合，红怒时前几张攻击牌免费打出。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class HailOfBladesPower : PowerBaseModel
{
 	private class Data
    {
        public int cardsPlayedThisTurn;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card))
        {
            return false;
        }

        modifiedCost = default(decimal);
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card))
        {
            return false;
        }

        modifiedCost = default(decimal);
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack && cardPlay.Card.EnergyCost.Canonical != 0 && cardPlay != null && !cardPlay.IsAutoPlay && cardPlay.IsLastInSeries)
        {
            GetInternalData<Data>().cardsPlayedThisTurn++;
        }

        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            GetInternalData<Data>().cardsPlayedThisTurn = 0;
        }

        return Task.CompletedTask;
    }

    private bool ShouldSkip(CardModel card)
    {
		// 如果卡牌不是自己的，则跳过
		if(card.Owner.Creature != base.Owner) return true;
		// 如果卡牌不是攻击牌，则跳过
		if(card.Type != CardType.Attack) return true;
		// 如果卡牌耗能为0，则跳过
		if(card.EnergyCost.Canonical == 0) return true;
		// 如果自己没有红怒，则跳过
		if(!base.Owner.HasPower<BerserkPower>()) return true;
		// 如果卡牌不在手牌或场上，则跳过
		if(card.Pile?.Type != PileType.Hand && card.Pile?.Type != PileType.Play) return true;

		// 如果已经打出的攻击牌数量大于等于本回合红怒时前几张攻击牌免费打出的数量，则跳过
        return GetInternalData<Data>().cardsPlayedThisTurn >= base.Amount;
    }
}