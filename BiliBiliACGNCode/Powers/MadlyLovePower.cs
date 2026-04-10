//****************** 代码文件申明 ***********************
//* 文件：MadlyLovePower(狂恋)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：你的病态会额外触发1次。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MadlyLovePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
}
