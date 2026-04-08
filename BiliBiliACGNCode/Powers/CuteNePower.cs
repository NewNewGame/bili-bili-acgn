//****************** 代码文件申明 ***********************
//* 文件：CuteNePower(可爱捏)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：女儿获得格挡时，给予你一半的格挡值。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class CuteNePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    /// <summary>
    /// 女儿获得格挡时，给予你一半的格挡值。
    /// </summary>
    /// <param name="creature"></param>
    /// <param name="amount"></param>
    /// <param name="props"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if(creature != base.Owner || creature.PetOwner == null) return;
        // 给予你一半的格挡值
        await CreatureCmd.GainBlock(creature.PetOwner.Creature, amount * Amount / 2, ValueProp.Unpowered, null);
    }

}
