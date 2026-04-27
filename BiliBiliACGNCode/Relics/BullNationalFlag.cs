//****************** 代码文件申明 ***********************
//* 文件：BullNationalFlag(牛国国旗)
//* 作者：wheat
//* 创建时间：2026/04/27 09:32:00 星期一
//* 描述：每场战斗中第一次进入红怒时，额外抽2张牌并额外获得2点能量。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class BullNationalFlag : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BerserkPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new EnergyVar(2),
    ];


    private bool _wasUsedThisCombat;

    private async Task TryTrigger()
    {
        // 如果已经触发过，则不触发
        if (_wasUsedThisCombat)
        {
            return;
        }
        // 设置为已触发
        _wasUsedThisCombat = true;
        // 闪烁
        Flash();
        // 获得2点能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        // 抽2张牌
        await CardPileCmd.Draw(CombatUtils.GetTemporaryPlayerChoiceContext(), base.DynamicVars.Cards.BaseValue, base.Owner);

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
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果对象不是自己，则返回
        if(power is BerserkPower && amount > 0 && power.Owner == base.Owner.Creature)
        {
            await TryTrigger();
        }
    }

}

