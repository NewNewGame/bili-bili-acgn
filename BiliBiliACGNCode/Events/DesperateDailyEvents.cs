using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class DesperateDailyEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Eat, "DESPERATE_DAILY.pages.INITIAL.options.EAT"),
            new EventOption(this, Leave, "DESPERATE_DAILY.pages.INITIAL.options.LEAVE")
        ];
    }

    private Task Eat()
    {
        // TODO: 实现选项 EAT 的具体逻辑
        return Task.CompletedTask;
    }

    private Task Leave()
    {
        // TODO: 实现选项 LEAVE 的具体逻辑
        return Task.CompletedTask;
    }
}
