//****************** 代码文件申明 ***********************
//* 文件：ChampionPowder(冠军粉)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：抽3/4张牌，选择1张牌将其打出2次。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class ChampionPowder : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DynamicVar("PlayTimes", 2m),
    ];

    public ChampionPowder() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 触发动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 抽牌
		var drawCards = await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
        HashSet<CardModel> hashSet = new HashSet<CardModel>(drawCards);
        if(hashSet.Count == 0) return;
        // 选择1张牌将其打出2次
		CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1){
            PretendCardsCanBePlayed = true
        };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		CardModel? card = (await CardSelectCmd.FromHand(choiceContext, base.Owner, prefs, (CardModel c) => hashSet.Contains(c) &!c.Keywords.Contains(CardKeyword.Unplayable), this)).FirstOrDefault();
		if (card != null)
		{
			await PowerCmd.Apply<ExtraPlayCardPower>(base.Owner.Creature, base.DynamicVars["PlayTimes"].IntValue-1, base.Owner.Creature, this);
			await AutoPlayUtils.AutoPlaySafely(choiceContext, card);
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["PlayTimes"].UpgradeValueBy(1m);
    }
}
