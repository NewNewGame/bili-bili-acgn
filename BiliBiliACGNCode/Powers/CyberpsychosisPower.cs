//****************** 代码文件申明 ***********************
//* 文件：CyberpsychosisPower(赛博精神病)
//* 作者：wheat
//* 创建时间：2026/05/05
//* 描述：本回合你抽到的卡牌费用都会被随机化。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class CyberpsychosisPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private int _testEnergyCostOverride = -1;
    public int TestEnergyCostOverride
	{
		get
		{
			return _testEnergyCostOverride;
		}
		set
		{
			TestMode.AssertOn();
			AssertMutable();
			_testEnergyCostOverride = value;
		}
	}

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if(card.Owner.Creature != base.Owner) return Task.CompletedTask;
        card.EnergyCost.SetThisTurnOrUntilPlayed(NextEnergyCost());
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        return Task.CompletedTask;
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == CombatSide.Player){
            await PowerCmd.Decrement(this);
        }
    }
    private int NextEnergyCost()
	{
		if (TestEnergyCostOverride >= 0)
		{
			return TestEnergyCostOverride;
		}
		return base.Owner.CombatState?.RunState.Rng.CombatEnergyCosts.NextInt(4) ?? TestEnergyCostOverride;
	}
}
