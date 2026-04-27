//****************** 代码文件申明 ***********************
//* 文件：StrangeMurmur
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：奇怪的低语
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class StrangeMurmur : EventBaseModel
{
    public override bool IsShared => true;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    public override EncounterModel? CanonicalEncounter => ModelDb.Encounter<StrangeMurmurEncounter>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("Relic", ModelDb.Relic<TurtleShell>().Title.GetFormattedText()),
    ];
    private static string MARIOGUIGUIPath => ImageHelper.GetImagePath("events/strange_murmur_guigui.png");
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return [
			new EventOption(this, Look, "STRANGE_MURMUR.pages.INITIAL.options.LOOK"),
			new EventOption(this, Ignore, "STRANGE_MURMUR.pages.INITIAL.options.IGNORE")
        ];
	}
    public override bool IsAllowed(IRunState runState){
        // 第一层限定
        return runState.CurrentActIndex == 0 && runState.TotalFloor >= 6;
    }
    /// <summary>
    /// 过去看看
    /// </summary>
    private Task Look(){
        // 设置事件场景图片
        if (LocalContext.IsMe(base.Owner))
		{
			NEventRoom.Instance.SetPortrait(PreloadManager.Cache.GetTexture2D(MARIOGUIGUIPath));
		}
        SetEventState(L10NLookup("STRANGE_MURMUR.pages.LOOK.description"), [
			new EventOption(this, Combat, "STRANGE_MURMUR.pages.LOOK.options.COMBAT", HoverTipFactory.FromRelic<TurtleShell>()),
		]);
        return Task.CompletedTask;
    }
    private Task Combat()
	{
        // 进入战斗，奖励龟壳和药水
		EnterCombatWithoutExitingEvent<StrangeMurmurEncounter>([
            new RelicReward(ModelDb.Relic<TurtleShell>().ToMutable(), base.Owner),
            new PotionReward(base.Owner)
        ], false);
        
		return Task.CompletedTask;
	}
    private Task Ignore()
    {
        SetEventFinished(L10NLookup("STRANGE_MURMUR.pages.IGNORE.END.description"));
        return Task.CompletedTask;
    }
}