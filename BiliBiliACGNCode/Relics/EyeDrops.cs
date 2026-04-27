//****************** 代码文件申明 ***********************
//* 文件：EyeDrops(眼药水)
//* 作者：wheat
//* 创建时间：2026/04/27 09:33:00 星期一
//* 描述：每场战斗中第一次获得的唐氏翻倍。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class EyeDrops : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TangShiPower>()];
    private bool _wasUsedThisCombat;
    /// <summary>
    /// 当唐氏层数增加时，触发
    /// </summary>
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is TangShiPower && amount > 0 && power.Owner == base.Owner.Creature)
        {
            if(_wasUsedThisCombat)
            {
                return;
            }
            _wasUsedThisCombat = true;
            Flash();
            await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, amount, base.Owner.Creature, cardSource);
        }
    }
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        _wasUsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _wasUsedThisCombat = false;
        return Task.CompletedTask;
    }
}

