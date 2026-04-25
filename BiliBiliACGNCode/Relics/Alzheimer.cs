//****************** 代码文件申明 ***********************
//* 文件：Alzheimer
//* 作者：wheat
//* 创建时间：2026/03/31 13:00:00 星期二
//* 描述：阿尔茨海默症 每场战斗开始时获得4点红温
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Core;
using BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class Alzheimer : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AngerPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("AngerAmount", 4m)];
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
        if(options.Any(option => option is AlzheimerRestSiteOption))
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
				await PowerCmd.Apply<AngerPower>(base.Owner.Creature, (int)base.DynamicVars["AngerAmount"].BaseValue, base.Owner.Creature, null);
			}
		}
	}
	/// <summary>
	/// 每打出一张牌时播放瓶子君语音
	/// </summary>
	/// <param name="context"></param>
	/// <param name="cardPlay"></param>
	/// <returns></returns>
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
		// 如果未启用角色语音，则不播放
		if(ACGNModConfig.UseCharacterVoice == false) return Task.CompletedTask;
		// 如果牌是瓶子君打出的并且卡牌为有一说一
		if(cardPlay.Card.Owner == base.Owner && cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY))
		{
			SfxCmd.Play(AudioUtils.BottleVoiceEventPath);
		}
		return Task.CompletedTask;
    }

}