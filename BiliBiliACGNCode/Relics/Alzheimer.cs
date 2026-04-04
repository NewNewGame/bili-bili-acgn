//****************** 代码文件申明 ***********************
//* 文件：Alzheimer
//* 作者：wheat
//* 创建时间：2026/03/31 13:00:00 星期二
//* 描述：阿尔茨海默症
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class Alzheimer : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
		options.Add(new AlzheimerRestSiteOption(player));
		return true;
	}
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		// 每场战斗开始时获得2点红温
		if (player == base.Owner)
		{
			CombatState combatState = player.Creature.CombatState;
			if (combatState.RoundNumber == 1)
			{
				Flash();
				await PowerCmd.Apply<AngerPower>(base.Owner.Creature, 2, base.Owner.Creature, null);
			}
		}
	}
}