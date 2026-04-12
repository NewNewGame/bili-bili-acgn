//****************** 代码文件申明 ***********************
//* 文件：LoveBrainPower(恋爱脑)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你给予 Amount 次病态后，获得 1 点能量（内部计数 TODO）
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class LoveBrainPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: Amount 为所需「给予病态」次数阈值；累计达标时获得 1 能量并重置计数
}
