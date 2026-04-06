//****************** 代码文件申明 ***********************
//* EvaEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：EVA事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class EvaEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Hp", 40m),
        new StringVar("Relic", ModelDb.Relic<AtFieldGenerator>().Title.GetFormattedText()),
        new StringVar("CardTitle", ModelDb.Card<EvaFormStrike>().Title),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var list = new List<EventOption>();
        if(base.Owner.Creature.CurrentHp <= base.Owner.Creature.MaxHp * base.DynamicVars["Hp"].BaseValue / 100m)
        {
            list.Add(new EventOption(this, null,"EVA_EVENTS.pages.INITIAL.options.LOCKED"));
        }
        else
        {
            list.Add(new EventOption(this, Try, "EVA_EVENTS.pages.INITIAL.options.TRY", HoverTipFactory.FromCard<EvaFormStrike>()));
        }
        list.Add(new EventOption(this, No, "EVA_EVENTS.pages.INITIAL.options.NO", HoverTipFactory.FromRelic<AtFieldGenerator>()));
        return list;
    }
    public override bool IsAllowed(RunState runState)
    {
        // 第一层限定
        return runState.TotalFloor <= EventUtils.FirstFloorMaxLevel;
    } 

    private async Task Try()
    {
        // 失去血量 
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.Owner.Creature.MaxHp * base.DynamicVars["Hp"].BaseValue / 100m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        // 获得卡牌
        CardModel card = base.Owner.RunState.CreateCard<EvaFormStrike>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        // 设置事件结束
        SetEventFinished(L10NLookup("EVA_EVENTS.pages.TRY.END.description"));
    }

    private async Task No()
    {
        // 获得遗物
        await RelicCmd.Obtain<AtFieldGenerator>(base.Owner);
        // 设置事件结束
        SetEventFinished(L10NLookup("EVA_EVENTS.pages.NO.END.description"));
    }
}
