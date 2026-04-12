//****************** 代码文件申明 ***********************
//* 文件：AnimeEmperorPower(动漫皇帝)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你激发 Amount 个充能球，随机生成 1 个充能球（内部计数 TODO）
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeEmperorPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: Amount 为每累计激发球数阈值；达标时随机 Channel 并扣减计数
}
