//****************** 代码文件申明 ***********************
//* 文件：WhirlwindBlock(旋风格挡)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：获得{Block:diff()}点[gold]格挡[/gold]。将弃牌堆中的一张牌放到抽牌堆顶部。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class WhirlwindBlock : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromOrb<BlockOrb>(), HoverTipFactory.Static(StaticHoverTip.Channeling)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BlockOrb", 1m)
    ];

    public WhirlwindBlock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 抽取1张牌
        var drawCards = await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        var cardModel = drawCards.FirstOrDefault();
        // 如果抽到的牌是技能牌，则生成1个格挡充能球
        if(cardModel != null && cardModel.Type == CardType.Skill){
            int cnt = base.DynamicVars["BlockOrb"].IntValue;
            for(int i = 0; i < cnt; i++){
                await OrbCmd.Channel<BlockOrb>(choiceContext, base.Owner);
                if(i < cnt - 1){
                    await OrbUtils.OrbChannelingWait();
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Block"].UpgradeValueBy(3m);
        base.DynamicVars["BlockOrb"].UpgradeValueBy(1m);
    }
}
