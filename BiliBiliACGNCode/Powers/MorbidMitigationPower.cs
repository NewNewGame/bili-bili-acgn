//****************** 代码文件申明 ***********************
//* 文件：MorbidMitigationPower(病态减免)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：本回合，受到来自病态的伤害减少。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MorbidMitigationPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    /// <summary>
    /// 回合结束时移除
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // 回合结束时移除
        if(side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
