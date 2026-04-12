//****************** 代码文件申明 ***********************
//* 文件：AloftThronePower(高高在上)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当女儿发动进攻时，抽 Amount 张牌
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AloftThronePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: 订阅女儿进攻事件，CardPileCmd.Draw
}
