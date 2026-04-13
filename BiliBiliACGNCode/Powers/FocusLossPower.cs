//****************** 代码文件申明 ***********************
//* 文件：FocusLossPower
//* 作者：wheat
//* 创建时间：2026/04/13
//* 描述：每回合结束时，失去{FocusLoss:diff()}点[gold]集中[/gold]。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;


//****************** 代码文件申明 ***********************
//* 文件：FocusLossPower
//* 作者：wheat
//* 创建时间：2026/04/13
//* 描述：每回合结束时，失去{FocusLoss:diff()}点[gold]集中[/gold]。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class FocusLossPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == CombatSide.Enemy){
            await PowerCmd.Apply<FocusPower>(base.Owner, -base.Amount, base.Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}