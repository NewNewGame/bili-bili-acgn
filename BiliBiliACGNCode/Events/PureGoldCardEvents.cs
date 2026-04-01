using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class PureGoldCardEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Leave, "PURE_GOLD_CARD.pages.INITIAL.options.LEAVE")
        ];
    }

    private Task Leave()
    {
        // TODO: 实现离开分支逻辑
        return Task.CompletedTask;
    }
}
