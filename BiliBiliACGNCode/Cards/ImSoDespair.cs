//****************** 代码文件申明 ***********************
//* 文件：ImSoDespair(我好绝望啊)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：丢弃你的所有[gold]手牌[/gold]。\n然后抽取相同数量的牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class ImSoDespair : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public ImSoDespair() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 丢弃所有手牌，然后抽取相同数量的牌
        var piles = PileType.Hand.GetPile(base.Owner).Cards;
        await CardCmd.DiscardAndDraw(choiceContext, piles, piles.Count());
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}
