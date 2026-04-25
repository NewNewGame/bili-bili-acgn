//****************** 代码文件申明 ***********************
//* 文件：OhHoHoHo(哦齁齁齁)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：本场战斗，你每生成过1个进攻充能球，就生成1个进攻充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class OhHoHoHo : CardBaseModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),  
        HoverTipFactory.FromOrb<AttackOrb>()
    ];

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new CalculationBaseVar(0m),
		new CalculationExtraVar(1m),
		new CalculatedVar("CalculatedChannels").WithMultiplier((CardModel card, Creature? _) => CombatManager.Instance.History.Entries.OfType<OrbChanneledEntry>().Count((OrbChanneledEntry e) => e.Actor.Player == card.Owner && e.Orb is AttackOrb))
	];

    public OhHoHoHo() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 本场战斗，你每生成过1个充能球，就生成1个充能球。
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		int cnt = (int)((CalculatedVar)base.DynamicVars["CalculatedChannels"]).Calculate(cardPlay.Target);
		for (int i = 0; i < cnt; i++)
		{
			await OrbCmd.Channel<AttackOrb>(choiceContext, base.Owner);
			if(i < cnt - 1)
			{
				await OrbUtils.OrbChannelingWait();
			}
		}
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}
