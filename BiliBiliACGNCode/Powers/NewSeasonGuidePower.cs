//****************** 代码文件申明 ***********************
//* 文件：NewSeasonGuidePower(新番导视)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：回合开始时触发最左侧充能球被动 Amount 次
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonGuidePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: 回合开始取队列首球 OrbCmd.Passive 循环 Amount 次
}
