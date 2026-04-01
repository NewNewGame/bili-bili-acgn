using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class AnimeTimeMachineEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "ANIME_TIME_MACHINE.pages.INITIAL.options.TRY"),
            new EventOption(this, No, "ANIME_TIME_MACHINE.pages.INITIAL.options.NO")
        ];
    }

    private Task Try()
    {
        // TODO: 实现选项 TRY 的具体逻辑
        return Task.CompletedTask;
    }

    private Task No()
    {
        // TODO: 实现选项 NO 的具体逻辑
        return Task.CompletedTask;
    }
}
