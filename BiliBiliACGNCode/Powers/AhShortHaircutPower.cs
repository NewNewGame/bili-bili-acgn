//****************** 代码文件申明 ***********************
//* 文件：AhShortHaircutPower(哎短发)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：回合开始时给予所有敌人 Amount 层病态
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AhShortHaircutPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: OnTurnStart / 等价钩子中对全体敌人 PowerCmd.Apply<MorbidPower>
}
