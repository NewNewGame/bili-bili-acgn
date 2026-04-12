//****************** 代码文件申明 ***********************
//* 文件：SailorUniformPower(水手服)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当女儿获得格挡时，对随机敌人造成 Amount 点伤害
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SailorUniformPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: 监听女儿获得格挡，DamageCmd 随机目标
}
