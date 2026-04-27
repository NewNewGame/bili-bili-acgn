//****************** 代码文件申明 ***********************
//* 文件：WoodenDoor
//* 作者：wheat
//* 创建时间：2026/04/27 16:15:30 星期一
//* 描述：事件：打不开的木门（掉血换随机遗物：概率/必得两种分支）。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class WoodenDoor : EventBaseModel
{
    public override bool IsShared => false;

    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar("HpLoss", 3m, ValueProp.Unblockable | ValueProp.Unpowered),
        new DynamicVar("Chance", 50m),
        new DamageVar("HpLoss2", 11m, ValueProp.Unblockable | ValueProp.Unpowered),
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, TryBreak, "WOODEN_DOOR.pages.INITIAL.options.TRY"),
            new EventOption(this, BreakWall, "WOODEN_DOOR.pages.INITIAL.options.BREAKWALL"),
        ];
    }

    public override bool IsAllowed(IRunState runState)
    {
        // 如果所有玩家的生命值都大于 HpLoss，则返回 true
        return runState.Players.All(player => player.Creature.CurrentHp > DynamicVars["HpLoss"].BaseValue);
    }

    private async Task TryBreak()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, (DamageVar)base.DynamicVars["HpLoss"], null, null);
        if(base.Owner.RunState.Rng.Niche.NextInt(0, 100) < base.DynamicVars["Chance"].IntValue)
        {
            RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
            await RelicCmd.Obtain(relic, base.Owner);
        }
        SetEventFinished(L10NLookup("WOODEN_DOOR.pages.TRY.END.description"));
    }

    private async Task BreakWall()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, (DamageVar)base.DynamicVars["HpLoss2"], null, null);
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
		await RelicCmd.Obtain(relic, base.Owner);
        SetEventFinished(L10NLookup("WOODEN_DOOR.pages.BREAKWALL.END.description"));
    }
}

