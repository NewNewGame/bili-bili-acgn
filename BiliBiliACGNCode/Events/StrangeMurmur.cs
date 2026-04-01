//****************** 代码文件申明 ***********************
//* 文件：StrangeMurmur
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：奇怪的低语
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class StrangeMurmur : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> {};
    public override EventLayoutType LayoutType => EventLayoutType.Default;
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return [
			new EventOption(this, Combat, "STRANGE_MURMUR.pages.INITIAL.options.COMBAT"),
			new EventOption(this, Ignore, "STRANGE_MURMUR.pages.INITIAL.options.IGNORE")
        ];
	}
    private Task Combat()
	{
        SetEventFinished(L10NLookup("STRANGE_MURMUR.pages.LEAVE.description"));
        /*
        // TODO 奖励龟壳，之后在做
		EnterCombatWithoutExitingEvent<StrangeMurmurEncounter>([
            new RelicReward(base.Owner),
            new PotionReward(base.Owner)
        ], false);
        */

		return Task.CompletedTask;
	}
    private Task Ignore()
    {
        SetEventFinished(L10NLookup("STRANGE_MURMUR.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}