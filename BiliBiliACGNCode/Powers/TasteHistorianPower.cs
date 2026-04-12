//****************** 代码文件申明 ***********************
//* 文件：TasteHistorianPower(品史官)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你激发 1 个充能球时，抽 Amount 张牌
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TasteHistorianPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: 监听充能球激发/被动，CardPileCmd.Draw
}
