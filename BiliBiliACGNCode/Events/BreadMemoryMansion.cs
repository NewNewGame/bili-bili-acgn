//****************** 代码文件申明 ***********************
//* 文件：BreadMemoryMansion
//* 作者：wheat
//* 创建时间：2026/05/04 21:00:00 星期一
//* 描述：面包记忆洋馆事件（Dio 问答分支，第二层触发）
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class BreadMemoryMansion : EventBaseModel
{
    public override bool IsShared => true;

    public override EventLayoutType LayoutType => EventLayoutType.Default;
    public override bool IsAllowed(IRunState runState) => runState.CurrentActIndex == 1;
    public override EncounterModel? CanonicalEncounter => ModelDb.Encounter<DioEncounter>();


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("Relic", ModelDb.Relic<DioVampireMask>().Title.GetFormattedText()),
        new DynamicVar("Hp", 20),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, DenyMemory, "BREAD_MEMORY_MANSION.pages.INITIAL.options.FORGET",
            HoverTipFactory.FromRelic<DioVampireMask>()),
        new EventOption(this, ExactCount, "BREAD_MEMORY_MANSION.pages.INITIAL.options.EXACT"),
    ];

    /// <summary>
    /// 选择「不记得」：进入 Dio 讥讽对白页，再进入战斗分支。
    /// </summary>
    private Task DenyMemory()
    {
        SetEventState(L10NLookup("BREAD_MEMORY_MANSION.pages.DIO_SCORN.description"), [
            new EventOption(this, StartDioCombat, "BREAD_MEMORY_MANSION.pages.DIO_SCORN.options.COMBAT",
                HoverTipFactory.FromRelic<DioVampireMask>()),
        ]);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 与 Dio 战斗；获胜应获得石鬼面（遗物 DioVampireMask）。
    /// </summary>
    private Task StartDioCombat()
    {
        // 进入战斗，奖励石鬼面和药水
		EnterCombatWithoutExitingEvent<DioEncounter>([
            new RelicReward(ModelDb.Relic<DioVampireMask>().ToMutable(), base.Owner),
            new PotionReward(base.Owner)
        ], false);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 报出 114514：饭局回复生命。
    /// </summary>
    private async Task ExactCount()
    {
        await CreatureCmd.Heal(base.Owner.Creature, (int)base.DynamicVars["Hp"].BaseValue, false);
        SetEventFinished(L10NLookup("BREAD_MEMORY_MANSION.pages.MEAL.END.description"));
    }
}
