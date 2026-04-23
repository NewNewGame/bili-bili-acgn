//****************** 代码文件申明 ***********************
//* DesperateDailyEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：绝望日常事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class DesperateDaily : EventBaseModel
{
    public override bool IsShared => false;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CurseTitle", ModelDb.Card<DespairSense>().Title),
        new StringVar("CurseTitle2", ModelDb.Card<EmptyStomach>().Title),
        new StringVar("Relic", ModelDb.Relic<QianHuDiary>().Title.GetFormattedText()),
    ];
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Eat, "DESPERATE_DAILY.pages.INITIAL.options.EAT", HoverTipFactory.FromCard<DespairSense>()),
            new EventOption(this, Leave, "DESPERATE_DAILY.pages.INITIAL.options.LEAVE", new IHoverTip[]{HoverTipFactory.FromCard<EmptyStomach>()}.Concat(HoverTipFactory.FromRelic<QianHuDiary>()).ToArray())
        ];
    }
    /// <summary>
    /// 如果玩家血量都小于等于30%那就可以进入
    /// </summary>
    /// <param name="runState"></param>
    /// <returns></returns>
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(player => player.Creature.CurrentHp <= player.Creature.MaxHp * 0.3m);
    } 

    private async Task Eat()
    {
        // 恢复50%点生命值，但获得诅咒【绝望感】。
        int maxHp = Mathf.Min(base.Owner.Creature.MaxHp - base.Owner.Creature.CurrentHp, (int)(base.Owner.Creature.MaxHp * 0.5m));
        await CreatureCmd.Heal(base.Owner.Creature, maxHp, false);
        CardModel card = base.Owner.RunState.CreateCard<DespairSense>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 1.2f);
        SetEventFinished(L10NLookup("DESPERATE_DAILY.pages.EAT.END.description"));
    }

    private async Task Leave()
    {
        // 获得诅咒【空腹】。获得千户的日记遗物
        CardModel card = base.Owner.RunState.CreateCard<EmptyStomach>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 1.2f);
        await RelicCmd.Obtain<QianHuDiary>(base.Owner);
        SetEventFinished(L10NLookup("DESPERATE_DAILY.pages.LEAVE.END.description"));
    }
}
