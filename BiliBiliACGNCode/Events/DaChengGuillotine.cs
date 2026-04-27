//****************** 代码文件申明 ***********************
//* 文件：DaChengGuillotine
//* 作者：wheat
//* 创建时间：2026/04/27 16:15:00 星期一
//* 描述：事件：大呈断头台（窃取藏宝图或庆祝选牌）。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class DaChengGuillotine : EventBaseModel
{
    public override bool IsShared => true;

    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardTitle", ModelDb.Card<SpoilsMap>().Title),
        new IntVar("FromCardChoiceCount", 5),
        new IntVar("CardChoiceCount", 1),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, TrySteal, "DA_CHENG_GUILLOTINE.pages.INITIAL.options.TRY", HoverTipFactory.FromCard<SpoilsMap>()),
            new EventOption(this, Celebrate, "DA_CHENG_GUILLOTINE.pages.INITIAL.options.CELEBRATE"),
        ];
    }

    public override bool IsAllowed(IRunState runState)
    {
        // 第一层限定
        return runState.CurrentActIndex == 0;
    }

    private async Task TrySteal()
    {
		CardModel card = base.Owner.RunState.CreateCard<SpoilsMap>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		await Cmd.CustomScaledWait(0.5f, 1.2f);
		SetEventFinished(L10NLookup("DA_CHENG_GUILLOTINE.pages.TRY.END.description"));
    }

    private async Task Celebrate()
    {
		Player owner = base.Owner;
		List<CardCreationResult> cards = CardFactory.CreateForReward(owner, base.DynamicVars["FromCardChoiceCount"].IntValue, CardCreationOptions.ForNonCombatWithDefaultOdds(new List<CardPoolModel>(){owner.Character.CardPool})).ToList();
		CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(L10NLookup("DA_CHENG_GUILLOTINE.pages.CELEBRATE.selectionScreenPrompt"), base.DynamicVars["CardChoiceCount"].IntValue){
            Cancelable = false,
        };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		CardModel cardModel = (await CardSelectCmd.FromSimpleGridForRewards(new BlockingPlayerChoiceContext(), cards, base.Owner, prefs)).FirstOrDefault();
		if (cardModel != null)
		{
			CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cardModel, PileType.Deck));
		}
		SetEventFinished(L10NLookup("DA_CHENG_GUILLOTINE.pages.CELEBRATE.END.description"));
    }
}

