//****************** 代码文件申明 ***********************
//* 文件：AnimeEmperorPower(动漫皇帝)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你激发 Amount 个充能球，随机生成 1 个充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeEmperorPower : PowerBaseModel
{
    private class Data
    {
        public int triggerCount;
    }
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    public override int DisplayAmount => base.Amount - GetInternalData<Data>().triggerCount;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
    ];


    protected override object InitInternalData()
    {
        return new Data();
    }
    /// <summary>
    /// 每当你激发 Amount 个充能球，随机生成 1 个充能球（内部计数 TODO）
    /// </summary>
    /// <param name="choiceContext">选择上下文</param>
    /// <param name="orb">充能球</param>
    /// <param name="targets">目标</param>
    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if(orb.Owner != base.Owner.Player) return;
        Data data = GetInternalData<Data>();
        data.triggerCount++;
        if(data.triggerCount >= base.Amount){
            data.triggerCount = 0;
            await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(base.Owner.CombatState), base.Owner.Player);
        }
    }
}
