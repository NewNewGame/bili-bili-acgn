//****************** 代码文件申明 ***********************
//* 文件：Distort(扭曲)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：给予女儿3/4层力量，并对随机敌人发动3/4次进攻。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Distort : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(CustomeHoverTips.AttackOrb)];


    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.RandomEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Strength", 3m),
        new DynamicVar("Attacks", 3m),
    ];

    public Distort() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给予女儿力量，然后对随机敌人发动多次攻击
        await DaughterCmd.ApplyPower<StrengthPower>(base.Owner.Creature, base.DynamicVars["Strength"].BaseValue, this);
        for(int i = 0; i < base.DynamicVars["Attacks"].BaseValue; i++){
            if(CombatState.HittableEnemies.Count == 0) break;
            await DaughterCmd.ApplyAttack(base.Owner.Creature, 0m, choiceContext, CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies));
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Strength"].UpgradeValueBy(1m);
        base.DynamicVars["Attacks"].UpgradeValueBy(1m);
    }
}
