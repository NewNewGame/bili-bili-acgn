//****************** 代码文件申明 ***********************
//* 文件：SuddenUpdate(突然更新)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：获得与你当前[gold]弃牌堆[/gold]牌数{IfUpgraded:show:+3|相等}的[gold]格挡[/gold]。{InCombat:\n(获得{CalculatedBlock:diff()}点[gold]格挡[/gold]。)|}
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class SuddenUpdate : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, creature) => 
        PileType.Discard.GetPile(card.Owner).Cards.Count())
    ];

    public SuddenUpdate() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得{CalculatedBlock:diff()}点[gold]格挡[/gold]。
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), base.DynamicVars.CalculatedBlock.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(3m);
    }
}
