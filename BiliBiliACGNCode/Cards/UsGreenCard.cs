//****************** 代码文件申明 ***********************
//* 文件：UsGreenCard
//* 作者：wheat
//* 创建时间：2026/03/31 10:23:13 星期二
//* 描述：自动打出手牌中所有带[gold]有一说一[/gold]的牌，并抽相同数量的牌。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using BiliBiliACGN.BiliBiliACGNCode.Utils;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class UsGreenCard : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Repeat", 1m)
    ];

    public UsGreenCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        #endregion
        // 获取所有带[gold]有一说一[/gold]的手牌
        var cards = PileType.Hand.GetPile(base.Owner).Cards.Where(card => card.Keywords.Contains(CustomKeyWords.YYSY)).ToArray();
        int n = cards.Count();
        // 遍历所有卡牌，自动打出带[gold]有一说一[/gold]的卡牌
        for(int i = 0; i < n; i++){
            await AutoPlayUtils.AutoPlaySafely(choiceContext, cards.ElementAt(i));
        }
        // 抽取相同数量的牌
        if(n > 0){
            await CardPileCmd.Draw(choiceContext, n, base.Owner);
        }
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.EnergyCost.UpgradeBy(-1);

        #endregion
    }
}
