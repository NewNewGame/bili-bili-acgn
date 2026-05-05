//****************** 代码文件申明 ***********************
//* 文件：Cyberpsychosis(赛博精神病)
//* 作者：wheat
//* 创建时间：2026/05/05
//* 描述：赛博精神病 当这张卡牌抽入你的手牌时，在本回合随机化你[gold]手牌[/gold]中所有牌的耗能。
//*******************************************************
using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class Cyberpsychosis : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Unplayable];
    private const int energyCost = -1;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;
    public override int MaxUpgradeLevel => 0;
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

    public Cyberpsychosis() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if(card == this){
            // 获取所有非X费卡牌然后随机
            IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => !c.EnergyCost.CostsX);
            foreach (CardModel item in enumerable)
            {
                if (item.EnergyCost.GetWithModifiers(CostModifiers.None) >= 0)
                {
                    item.EnergyCost.SetThisTurnOrUntilPlayed(NextEnergyCost());
                    NCard.FindOnTable(item)?.PlayRandomizeCostAnim();
                }
            }
            // 获得赛博精神病
            await PowerCmd.Apply<CyberpsychosisPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        }
    }
    private int NextEnergyCost()
	{
		if (TestEnergyCostOverride >= 0)
		{
			return TestEnergyCostOverride;
		}
		return base.Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
	}
}
