//****************** 代码文件申明 ***********************
//* 文件：EmergencyOxygen(紧急吸氧)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：打出1/2张有一说一,丢弃1/2张牌。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using BiliBiliACGN.BiliBiliACGNCode.Utils;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class EmergencyOxygen : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CustomKeyWords.YYSY),
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
    ];

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public EmergencyOxygen() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 打出1/2张有一说一
        int cardCount = (int)base.DynamicVars.Cards.BaseValue;
        var cards = await CardSelectCmd.FromHand(choiceContext, base.Owner, new CardSelectorPrefs(MCardSelectorPrefs.TO_YYSY, cardCount), MCardSelectorPrefs.YYSYFilter, this);
        foreach(var card in cards){
            await AutoPlayUtils.AutoPlaySafely(choiceContext, card);
        }
        // 丢弃1/2张牌
        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, cardCount), null, this));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
