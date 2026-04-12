//****************** 代码文件申明 ***********************
//* 文件：DestinyStagePower(命运舞台)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你打出能力牌时，随机生成 Amount 个充能球
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class DestinyStagePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: AfterCardPlayed 能力牌 -> OrbCmd.Channel 随机球
}
