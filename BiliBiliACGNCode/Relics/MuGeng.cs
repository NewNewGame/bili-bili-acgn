//****************** 代码文件申明 ***********************
//* 文件：MuGeng(木更)
//* 作者：wheat
//* 创建时间：2026/04/27 09:36:00 星期一
//* 描述：你每次给予敌人病态时，额外再给予1层病态。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(FunShikiRelicPool))]
public sealed class MuGeng : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Morbid", 1m),
    ];
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果对象不是敌人，则返回
        if(power.Owner.Side != CombatSide.Enemy)
        {
            return;
        }
        // 如果不是自己给的，则返回
        if(applier != base.Owner.Creature){
            return;
        }
        // 如果层数小于等于0，则返回
        if(amount <= 0){
            return;
        }

        // 给予敌人1层病态
        await PowerCmd.Apply<MorbidPower>(power.Owner, base.DynamicVars["Morbid"].BaseValue, base.Owner.Creature, cardSource);
    }

}

