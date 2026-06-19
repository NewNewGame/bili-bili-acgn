//****************** 代码文件申明 ***********************
//* 文件：TwistPower(扭曲)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：你下一次卡牌给予的 debuff 额外给予 1 点
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TwistPower : PowerBaseModel
{
	private class Data
	{
		public CardModel? cardSource;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override object InitInternalData()
	{
		return new Data();
	}
    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        // 如果目标为空，则返回
        if(target == null) return 0;
        // 如果卡牌来源为空，则返回
        if(cardSource == null) return 0;
        // 如果卡牌来源为诅咒或者状态牌，则返回（免得卡牌信息变了）
        if(cardSource.Type == CardType.Curse || cardSource.Type == CardType.Status) return 0;
        // 如果不是自己给的debuff，则返回
        if(cardSource.Owner.Creature != base.Owner || power.Type != PowerType.Debuff) return 0;
        // 如果目标是友军，则返回
        if(target.Side == base.Owner.Side) return 0;
        Data internalData = GetInternalData<Data>();
        if(internalData.cardSource != null && internalData.cardSource != cardSource) return 0;
        internalData.cardSource = cardSource;
        return base.Amount;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner != base.Owner.Player) return;
        Data internalData = GetInternalData<Data>();
        if(internalData.cardSource != null && internalData.cardSource != cardPlay.Card) return;
        if(internalData.cardSource == cardPlay.Card && cardPlay.IsLastInSeries){
            await PowerCmd.Remove(this);
        }
    }

}
