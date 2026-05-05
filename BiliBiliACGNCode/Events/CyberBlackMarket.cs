//****************** 代码文件申明 ***********************
//* 文件：CyberBlackMarket
//* 作者：wheat
//* 创建时间：2026/05/04 12:00:00 星期一
//* 描述：夜之城义体医生黑市事件（斯安威斯坦 / 赛博精神病分支占位）
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class CyberBlackMarket : EventBaseModel
{
    public override bool IsShared => false;

    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("Relic", ModelDb.Relic<SandevistanRelic>().Title.GetFormattedText()),
        new StringVar("CardTitle", ModelDb.Card<Cyberpsychosis>().Title),
        new DynamicVar("Chance", 30),
        new DynamicVar("Gold", 50),
        new DynamicVar("Gold2", 100),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, Tempted, "CYBER_BLACK_MARKET.pages.INITIAL.options.TEMPT",
            HoverTipFactory.FromRelic<SandevistanRelic>().Concat(new []{HoverTipFactory.FromCard<Cyberpsychosis>()}).ToArray()),
        new EventOption(this, DeliverContract, "CYBER_BLACK_MARKET.pages.INITIAL.options.DELIVER"),
    ];

    private async Task Tempted()
    {
        // 获得斯安威斯坦遗物
        await RelicCmd.Obtain<SandevistanRelic>(base.Owner);
        // 按设计概率判定，若成功则获得赛博精神病
        if(base.Owner.RunState.Rng.Niche.NextInt(0, 100) < (int)base.DynamicVars["Chance"].BaseValue)
        {
            // 获得赛博精神病
            CardModel card = base.Owner.RunState.CreateCard<Cyberpsychosis>(base.Owner);
            CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        }
        SetEventFinished(L10NLookup("CYBER_BLACK_MARKET.pages.TEMPT.END.description"));
    }

    private Task DeliverContract()
    {
        PlayerCmd.GainGold(base.Owner.RunState.Rng.Niche.NextInt((int)base.DynamicVars["Gold"].BaseValue, (int)base.DynamicVars["Gold2"].BaseValue), base.Owner);
        SetEventFinished(L10NLookup("CYBER_BLACK_MARKET.pages.DELIVER.END.description"));
        return Task.CompletedTask;
    }
}
