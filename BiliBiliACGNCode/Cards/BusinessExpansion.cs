//****************** 代码文件申明 ***********************
//* 文件：BusinessExpansion
//* 作者：wheat
//* 创建时间：2026/03/31 08:54:54 星期二
//* 描述：在本场战斗中，为手牌中{Cards:diff()}张牌添加[gold]有一说一[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BusinessExpansion : CardBaseModel
{
    // 消耗
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY)];

    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public BusinessExpansion() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果

        #endregion
        // 计数手牌中有多少个没有有一说一的牌
        int count = PileType.Hand.GetPile(base.Owner).Cards.Count(c => !c.Keywords.Contains(CustomKeyWords.YYSY));
        int num = Mathf.Min((int)base.DynamicVars.Cards.BaseValue, count);
        if(num == 0)
        {
            return;
        }
        var cards = await CardSelectCmd.FromHand(choiceContext, base.Owner, new CardSelectorPrefs(MCardSelectorPrefs.TO_ADD_YYSY, num), MCardSelectorPrefs.NoYYSYFilter, this);
        foreach(var card in cards){
            card.AddKeyword(CustomKeyWords.YYSY);
        }
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Cards"].UpgradeValueBy(1);

        #endregion
    }
}
