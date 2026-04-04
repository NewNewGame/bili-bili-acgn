//****************** 代码文件申明 ***********************
//* 文件：NuclearWeaponEvent
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：核武器！事件（第二层优先；实现保留）
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Potions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class Nuke : EventBaseModel
{
    public override bool IsShared => true;

    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CurseTitle", ModelDb.Card<NuclearRadiation>().Title),
        new StringVar("Potion", ModelDb.Potion<HextechBomb>().Title.GetFormattedText()),
    ];
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Follow, "NUCLEAR_WEAPON.pages.INITIAL.options.FOLLOW", HoverTipFactory.FromCard<NuclearRadiation>()),
            new EventOption(this, StopKids, "NUCLEAR_WEAPON.pages.INITIAL.options.STOP", HoverTipFactory.FromPotion<HextechBomb>()),
        ];
    }
    public override bool IsAllowed(RunState runState){
        // 第二层限定
        return runState.TotalFloor <= EventUtils.SecondFloorMaxLevel && runState.TotalFloor >= EventUtils.FirstFloorMaxLevel;
    }

    private async Task Follow()
    {
        // 获得诅咒 核辐射
        CardModel card = base.Owner.RunState.CreateCard<NuclearRadiation>(base.Owner);
		CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        // 移除2张牌
        CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, 2){
            Cancelable = true,
            RequireManualConfirmation = true,
        };
        // 选择2张牌
        CardSelectorPrefs prefs = cardSelectorPrefs;
        IEnumerable<CardModel> enumerable = await CardSelectCmd.FromDeckForRemoval(base.Owner, prefs);
        if (enumerable.Any())
        {
            foreach (CardModel item in enumerable)
            {
                await CardPileCmd.RemoveFromDeck(item);
            }
        }
        SetEventFinished(L10NLookup("NUCLEAR_WEAPON.pages.FOLLOW.END.description"));
    }

    private async Task StopKids()
    {
        // 获得药水【海克斯炸弹】
        await RewardsCmd.OfferCustom(base.Owner, new List<Reward> { new PotionReward(ModelDb.Potion<HextechBomb>(), base.Owner) });
        SetEventFinished(L10NLookup("NUCLEAR_WEAPON.pages.STOP.END.description"));
    }

}

