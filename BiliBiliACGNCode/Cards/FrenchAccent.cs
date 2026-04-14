//****************** 代码文件申明 ***********************
//* 文件：FrenchAccent(法国口音)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：给予女儿{Strength:diff()}点[gold]力量[/gold]。抽{Cards:diff()}张牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class FrenchAccent : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Strength", 1m),
        new CardsVar(2)
    ];

    public FrenchAccent() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 给予女儿{Strength:diff()}点[gold]力量[/gold]。抽取{Cards:diff()}张牌。
        await DaughterCmd.ApplyPower<StrengthPower>(base.Owner.Creature, base.DynamicVars["Strength"].BaseValue, this);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars["Cards"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Strength"].UpgradeValueBy(1m);
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
