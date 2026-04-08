//****************** 代码文件申明 ***********************
//* 文件：TelecomEngineeringMasterPower(通信工程硕士)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：你每激发1个充能球，获得1点集中。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TelecomEngineeringMasterPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.FromPower<FocusPower>()];

    /// <summary>
    /// 每激发1个充能球，获得1点集中。
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="orb"></param>
    /// <param name="targets"></param>
    /// <returns></returns>
    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        // 如果充能球所有者不是你，则返回
        if(orb.Owner.Creature != base.Owner) return;
        // 获得集中
        await PowerCmd.Apply<FocusPower>(base.Owner, Amount, base.Owner, null);
    }

}
