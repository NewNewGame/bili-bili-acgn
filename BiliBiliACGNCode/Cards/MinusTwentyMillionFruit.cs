//****************** 代码文件申明 ***********************
//* 文件：MinusTwentyMillionFruit(负2000万果)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：生成3/4个力量充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class MinusTwentyMillionFruit : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling),HoverTipFactory.FromOrb<StrengthOrb>()];
    #endregion  
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("OrbCount", 3)
    ];

    public MinusTwentyMillionFruit() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 随机生成{OrbCount:diff()}个力量充能球
        int orbCount = base.DynamicVars["OrbCount"].IntValue;
        for(int i = 0; i < orbCount; i++)
        {
            await OrbCmd.Channel<StrengthOrb>(choiceContext, base.Owner);
            if(i < orbCount - 1)
            {
                await OrbUtils.OrbChannelingWait();
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["OrbCount"].UpgradeValueBy(1m);
    }
}
