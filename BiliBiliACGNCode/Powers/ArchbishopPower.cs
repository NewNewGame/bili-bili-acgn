//****************** 代码文件申明 ***********************
//* 文件：ArchbishopPower(大主教)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：本回合女儿每次攻击指定敌人时，你获得 Amount 点力量（关联目标与攻击事件 TODO）
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ArchbishopPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    // TODO: 记录打出大主教时选中的敌人；监听女儿对该敌人的攻击并施加力量；回合结束移除
}
