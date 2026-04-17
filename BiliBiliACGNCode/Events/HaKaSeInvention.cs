//****************** 代码文件申明 ***********************
//* HaKaSeInventionEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：博士发明事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class HaKaSeInvention : EventBaseModel
{
    public override bool IsShared => false;
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new GoldVar(50),
        new StringVar("Relic", ModelDb.Relic<MildSneezing>().Title.GetFormattedText()),
        new StringVar("Relic2", ModelDb.Relic<HaKaSeSneezingMachine>().Title.GetFormattedText()),
    ];
    public override bool IsAllowed(IRunState runState){
        // 大于100块钱
        foreach(var player in runState.Players){
            if(player.Gold < 100){
                return false;
            }
        }
        return true;
    }
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>[
        new EventOption(this, Try, "HA_KA_SE_INVENTION.pages.INITIAL.options.TRY", HoverTipFactory.FromRelic<MildSneezing>()),
        new EventOption(this, Buy, "HA_KA_SE_INVENTION.pages.INITIAL.options.BUY", HoverTipFactory.FromRelic<HaKaSeSneezingMachine>())
    ];
    private async Task Try()
    {
        // 获得轻微喷嚏遗物
        await RelicCmd.Obtain<MildSneezing>(base.Owner);
        // 设置事件结束
        SetEventFinished(L10NLookup("HA_KA_SE_INVENTION.pages.TRY.END.description"));
    }

    private async Task Buy()
    {
        // 获得博士的喷嚏机遗物
        await RelicCmd.Obtain<HaKaSeSneezingMachine>(base.Owner);
        await PlayerCmd.LoseGold(50, base.Owner);
        // 设置事件结束
        SetEventFinished(L10NLookup("HA_KA_SE_INVENTION.pages.BUY.END.description"));
    }
}
