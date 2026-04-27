//****************** 代码文件申明 ***********************
//* 文件：StartUp
//* 作者：wheat
//* 创建时间：2026/04/27 17:30:00 星期一
//* 描述：启动 战斗开始时，将这张卡牌加入到你的手牌
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Enchantments;

public sealed class StartUp : EnchantmentBaseModel
{
    public override bool ShouldStartAtBottomOfDrawPile => true;

	public override bool ShowAmount => false;

	public override bool CanEnchantCardType(CardType cardType)
	{
		return true;
	}

	public override async Task BeforePlayPhaseStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == base.Card.Owner && base.Card.CombatState.RoundNumber == 1)
		{
            await CardPileCmd.Add(base.Card, PileType.Hand);
		}
	}
}