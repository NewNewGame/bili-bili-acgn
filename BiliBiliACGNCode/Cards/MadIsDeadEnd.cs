//****************** 代码文件申明 ***********************
//* 文件：MadIsDeadEnd(做mad死路一条)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成本场战斗你生成过的充能球2/3倍的伤害。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
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
public sealed class MadIsDeadEnd : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];

    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Multiplier", 2m),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedChannels").WithMultiplier((CardModel card, Creature? _) => CombatManager.Instance.History.Entries.OfType<OrbChanneledEntry>().Count((OrbChanneledEntry e) => e.Actor.Player == card.Owner))
    ];

    public MadIsDeadEnd() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 按“本场战斗已生成的充能球数量 * 倍率”对所有敌人造成伤害
        int channels = (int)((CalculatedVar)base.DynamicVars["CalculatedChannels"]).Calculate(cardPlay.Target);
        await DamageCmd.Attack(channels * base.DynamicVars["Multiplier"].BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }
}
