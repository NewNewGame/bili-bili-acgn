//****************** 代码文件申明 ***********************
//* 文件：UnreachableFlower(高岭之花)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：给予敌人6/9层病态，抽2/3张牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class UnreachableFlower : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Morbid", 8m),
        new CardsVar(2),
    ];

    public UnreachableFlower() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.CombatState == null) return;
        // 给予敌人6/9层病态
        await PowerCmd.Apply<MorbidPower>(cardPlay.Target, base.DynamicVars["Morbid"].BaseValue, base.Owner.Creature, this);
        // 抽2/3张牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade() { 
        base.DynamicVars["Morbid"].UpgradeValueBy(3m);
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
