//****************** 代码文件申明 ***********************
//* 文件：Vlog(Vlog)
//* 作者：wheat
//* 创建时间：2026/04/27 09:40:00 星期一
//* 描述：当敌人的病态层数大于等于其生命值时，它造成的伤害降低60%。
//*******************************************************

using BaseLib.Extensions;
using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(FunShikiRelicPool))]
public sealed class Vlog : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Decrease", 60m),
    ];
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 如果目标不是自己，则返回
        if(target != base.Owner.Creature) return 1m;
        // 如果伤害不是有攻击力造成的，则返回
        if(!props.IsPoweredAttack_()) return 1m;
        // 如果攻击者不是敌人，则返回
        if(dealer == null || dealer.Side != CombatSide.Enemy) return 1m;
        // 如果敌人的病态层数小于其生命值，则返回
        if(dealer.GetPowerAmount<MorbidPower>() < target.CurrentHp) return 1m;
        return 1m - base.DynamicVars["Decrease"].BaseValue / 100m;
    }


}

