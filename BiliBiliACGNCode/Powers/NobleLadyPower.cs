//****************** 代码文件申明 ***********************
//* 文件：NobleLadyPower(贵族小姐)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你受到伤害，获得1层病态。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NobleLadyPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

}
