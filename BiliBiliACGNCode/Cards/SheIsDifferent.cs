//****************** 代码文件申明 ***********************
//* 文件：SheIsDifferent(她不一样)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：给予所有敌人4/7层病态和2/3层虚弱。消耗。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class SheIsDifferent : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>(),HoverTipFactory.FromPower<WeakPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MorbidStacks", 4m),
        new DynamicVar("WeakStacks", 2m),
    ];

    public SheIsDifferent() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.CombatState == null) return;
        var enemies = base.CombatState.HittableEnemies;
        foreach(var enemy in enemies){
            await PowerCmd.Apply<MorbidPower>(enemy, base.DynamicVars["MorbidStacks"].BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(enemy, base.DynamicVars["WeakStacks"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["MorbidStacks"].UpgradeValueBy(3m);
        base.DynamicVars["WeakStacks"].UpgradeValueBy(1m);
    }
}
