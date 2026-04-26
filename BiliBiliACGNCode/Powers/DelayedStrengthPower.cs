//****************** 代码文件申明 ***********************
//* 文件：DelayedStrengthPower(延迟力量)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：在你下一个回合开始时，获得力量（数值见 Amount / CanonicalVars）。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class DelayedStrengthPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if(side == CombatSide.Player)
        {
            await PowerCmd.Apply<StrengthPower>(base.Owner, Amount, base.Owner, null);
            await PowerCmd.Remove(this);
        }
    }

}
