//****************** 代码文件申明 ***********************
//* 文件：Roar(怒吼)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：获得{Block:diff()}点[gold]格挡[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class Roar : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move)
    ];

    public Roar() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 Block 格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, base.DynamicVars.Block.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Block"].UpgradeValueBy(2m);
        base.AddKeyword(CustomKeyWords.YYSY);
    }
}
