//****************** 代码文件申明 ***********************
//* 文件：AnimeMasterPower(动漫高手)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：女儿现在会攻击所有敌人。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeMasterPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
