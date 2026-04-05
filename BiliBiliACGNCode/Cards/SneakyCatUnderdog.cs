//****************** 代码文件申明 ***********************
//* 文件：SneakyCatUnderdog(偷腥猫（败犬）)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：将你弃牌堆中所有稀有牌移入手牌。保留，消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class SneakyCatUnderdog : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public SneakyCatUnderdog() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 遍历弃牌堆，筛选 CardRarity.Rare 移入手牌
        IEnumerable<CardModel> cards = PileType.Discard.GetPile(base.Owner).Cards.Where((CardModel c) => c.Rarity == CardRarity.Rare).ToList();
		await CardPileCmd.Add(cards, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        base.AddKeyword(CardKeyword.Retain);
    }
}
