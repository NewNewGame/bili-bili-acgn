//****************** 代码文件申明 ***********************
//* 文件：AddMaxHpTempPower
//* 作者：wheat
//* 创建时间：2026/04/09
//* 描述：临时提升最大生命值
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;
public sealed class AddMaxHpTempPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果不是本能力，对象不是自己，则返回
        if(power != this || power.Owner != base.Owner) return;

        // 如果数量小于等于0，则返回
        if(amount <= 0) return;

        // 计算当前生命值和最大生命值
        int curHp = base.Owner.CurrentHp;
        if(curHp + amount > base.Owner.MaxHp){
            await CreatureCmd.SetMaxHp(base.Owner, curHp + amount);
        }
        // 治疗生命值
        await CreatureCmd.Heal(base.Owner, amount, true);
    }
    /// <summary>
    /// 受到伤害时，移除给予的最大生命值
    /// </summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if(target != base.Owner) return;
        int amount = Mathf.Min(this.Amount, result.TotalDamage);
        await PowerCmd.Apply<AddMaxHpTempPower>(base.Owner, -amount, base.Owner, null);
        await CreatureCmd.SetMaxHp(base.Owner, Mathf.Max(base.Owner.MaxHp - amount, 1));
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == CombatSide.Enemy){
            // 如果女儿有动漫高手能力，则不移除给予的最大生命值
            if(base.Owner.HasPower<AnimeMasterPower>()) return;
            // 移除给予的最大生命值，保证生命值不会低于1
            await CreatureCmd.SetMaxHp(base.Owner, Mathf.Max(base.Owner.MaxHp - this.Amount, 1));
            await PowerCmd.Remove(this);
        }
    }
}