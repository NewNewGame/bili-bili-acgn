//****************** 代码文件申明 ***********************
//* 文件：TwistPower(扭曲)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：你的下一次给予 debuff 额外给予 1 点（层数由 Amount 表示，具体结算 TODO）
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

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        // 如果不是自己给的power，或者不是debuff，则返回
        if(giver != base.Owner || power.Type != PowerType.Debuff) return amount;
        // 如果目标是友军，则返回
        if(target != null && target.Side == base.Owner.Side) return amount;
        Data internalData = GetInternalData<Data>();
        if(internalData.cardSource != null && internalData.cardSource != cardSource) return amount;
        internalData.cardSource = cardSource;
        return amount + base.Amount;
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
