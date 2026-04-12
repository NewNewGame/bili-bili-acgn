//****************** 代码文件申明 ***********************
//* 文件：TwistBuffPower(扭曲)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：你的下一次给予 debuff 额外给予 1 点（层数由 Amount 表示，具体结算 TODO）
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TwistBuffPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: 在下一次对敌人施加 debuff 时额外附加层数，并消耗/减少 Amount
}
