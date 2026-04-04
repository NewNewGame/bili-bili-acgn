//****************** 代码文件申明 ***********************
//* BagaMegamiEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：智障女神事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class BagaMegami : EventBaseModel
{
    public override bool IsShared => false;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CardTitle", ModelDb.Card<AquasBlessing>().Title),
        new StringVar("Relic", ModelDb.Relic<AquasTears>().Title.GetFormattedText()),
        new StringVar("Relic2", ModelDb.Relic<AquaCompanion>().Title.GetFormattedText()),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "BAGA_MEGAMI_.pages.INITIAL.options.TRY", HoverTipFactory.FromCard<AquasBlessing>()),
            new EventOption(this, No, "BAGA_MEGAMI_.pages.INITIAL.options.NO", HoverTipFactory.FromRelic<AquasTears>()),
            new EventOption(this, Megami, "BAGA_MEGAMI_.pages.INITIAL.options.MEGAMI", HoverTipFactory.FromRelic<AquaCompanion>())
        ];
    }

    private async Task Try()
    {
        // 让她加buff 获得阿库娅的祝福卡牌
        CardModel card = base.Owner.RunState.CreateCard<AquasBlessing>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        SetEventFinished(L10NLookup("BAGA_MEGAMI_.pages.TRY.END.description"));
    }

    private async Task No()
    {
        // 拒绝她，获得阿库娅的眼泪
        await RelicCmd.Obtain<AquasTears>(base.Owner);
        SetEventFinished(L10NLookup("BAGA_MEGAMI_.pages.NO.END.description"));
    }

    private async Task Megami()
    {
        // 加入队伍，获得智障女神同行遗物
        await RelicCmd.Obtain<AquaCompanion>(base.Owner);
        SetEventFinished(L10NLookup("BAGA_MEGAMI_.pages.MEGAMI.END.description"));
    }
}
