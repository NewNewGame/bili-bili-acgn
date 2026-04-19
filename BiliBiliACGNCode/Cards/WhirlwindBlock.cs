//****************** 代码文件申明 ***********************
//* 文件：WhirlwindBlock(旋风格挡)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：抽取1张牌。如果抽到的牌是技能牌，则[gold]生成[/gold]{BlockOrb:diff()}个[gold]生命[/gold]充能球。
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
public sealed class WhirlwindBlock : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LifeOrb>(),
    ];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
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
        // 如果抽到的牌是技能牌，则生成1个生命充能球
        if(cardModel != null && cardModel.Type == CardType.Skill){
            int cnt = base.DynamicVars["BlockOrb"].IntValue;
            for(int i = 0; i < cnt; i++){
                await OrbCmd.Channel<LifeOrb>(choiceContext, base.Owner);
                if(i < cnt - 1){
                    await OrbUtils.OrbChannelingWait();
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BlockOrb"].UpgradeValueBy(1m);
    }
}
