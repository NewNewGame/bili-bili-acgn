//****************** 代码文件申明 ***********************
//* 文件：MadIsDeadEnd(做mad死路一条)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成本场战斗你生成过的充能球2/3倍的伤害。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class MadIsDeadEnd : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Multiplier", 2m),
    ];

    public MadIsDeadEnd() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 按“本场战斗已生成的充能球数量 * 倍率”对所有敌人造成伤害
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }
}
