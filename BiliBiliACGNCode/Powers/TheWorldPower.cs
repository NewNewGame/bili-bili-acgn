//****************** 代码文件申明 ***********************
//* TheWorldPower
//* 作者：wheat
//* 创建时间：2026/05/04 21:00:00 星期一
//* 描述：世界，本回合打出{Amount}张牌后，结束回合。
//*******************************************************

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TheWorldPower : PowerBaseModel
{
    private class Data
	{
        public int playedCards;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => Mathf.Max(0, base.Amount - GetInternalData<Data>().playedCards);

	protected override object InitInternalData()
	{
		return new Data();
	}
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player != base.Owner.Player) return Task.CompletedTask;
        GetInternalData<Data>().playedCards = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // 敌方回合结束时，重置打出的牌数
        if(side == CombatSide.Enemy){
            GetInternalData<Data>().playedCards = 0;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 不是自己打的牌，不处理
        if(cardPlay.Card.Owner.Creature != base.Owner) return Task.CompletedTask;
        // 玩家不存在，不处理
        if(base.Owner.Player == null) return Task.CompletedTask;
        // 更新打出的牌数
        Data internalData = GetInternalData<Data>();
        // 以防某些特殊原因连环打出，多次触发。
        if(internalData.playedCards >= base.Amount) return Task.CompletedTask;
        internalData.playedCards++;
        InvokeDisplayAmountChanged();
        // 打出的牌数达到Amount，结束回合
        if(internalData.playedCards >= base.Amount)
        {
            Flash();
            PlayerCmd.EndTurn(base.Owner.Player, false);
        }
        return Task.CompletedTask;
    }
}