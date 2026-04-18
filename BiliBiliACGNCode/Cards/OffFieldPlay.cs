//****************** 代码文件申明 ***********************
//* 文件：OffFieldPlay(就玩场外)
//* 作者：wheat
//* 创建时间：2026/03/31 10:22:52 星期二
//* 描述：抽牌直至你的[gold]手牌[/gold]有{Cards:diff()}张牌。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class OffFieldPlay : CardBaseModel
{
    #region 卡牌关键词与悬停
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(6)
    ];

    public OffFieldPlay() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        #endregion
        // 计算需要抽取的牌数
        int cnt = base.DynamicVars.Cards.IntValue - PileType.Hand.GetPile(base.Owner).Cards.Count;
        if(cnt <= 0) return;
        await CardPileCmd.Draw(choiceContext, cnt, base.Owner);
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Cards"].UpgradeValueBy(1m);

        #endregion
    }
}
