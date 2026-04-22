//****************** 代码文件申明 ***********************
//* 文件：MorbidPower(病态)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当你对敌方造成伤害时，自身受到病态层数的伤害。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.GameActions.Multiplayer;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MorbidPower : PowerBaseModel
{
    public const ValueProp MORBID_VALUE_PROP = ValueProp.Unpowered | ValueProp.Unblockable;
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageReductionPercent", 30m),
    ];
    /// <summary>
    /// 预估下一回合造成的伤害
    /// 不精准
    /// </summary>
    /// <returns></returns>
    public int CalculateTotalDamageNextTurn()
	{
        // 如果战斗状态为空，则返回Amount
        if(base.Owner.CombatState == null) return Amount;
        // 如果施加者为玩家或宠物，则返回Amount
        if(base.Owner.IsPlayer || base.Owner.IsPet) return Amount;
        // 如果施加者为敌人，则计算伤害
		decimal num = default(decimal);
        // 计算触发次数
        int num2 = 1 + base.Owner.GetPowerAmount<MadlyLovePower>();
        // 计算攻击次数，默认1次
        int atkTimes = 1;
        // 从攻击意图中获取攻击次数
        if(base.Owner.Monster != null){
            foreach (AbstractIntent intent in base.Owner.Monster.NextMove.Intents)
            {
                if (intent is AttackIntent atk)
                {
                    atkTimes = atk.Repeats;
                    break;
                }
            }
        }
        int limit = int.MaxValue;
        // 如果持有者难以杀死，则限制为硬到死层数
        if(base.Owner.HasPower<HardToKillPower>()){
            limit = base.Owner.GetPowerAmount<HardToKillPower>();
        }
        // 如果持有者无形，则限制为1层
        if(base.Owner.HasPower<IntangiblePower>()){
            limit = 1;
        }
        // 减免计数
        decimal reduction = 0;
        // 剩余病态层数
        decimal amt = Amount;
        // 获取所有敌人
        IEnumerable<Creature> source = from c in base.Owner.CombatState.GetOpponentsOf(base.Owner) where c.IsAlive select c;
        // 遍历所有敌人
        foreach(var creature in source){
            // 如果敌人有痴迷对象，则计算伤害
            if(creature.HasPower<InfatuationTargetPower>()){
                // 每次攻击叠加计算
                for(int _ = 0; _ < atkTimes; _++){
                    // 单次伤害触发次数
                    for(int __ = 0; __ < num2; __++){
                        // 计算伤害，累加减免
                        num += Mathf.Min(limit, (int)((100m - reduction) / 100m * amt));
                        // 如果触发次数用完了，则退出循环
                        if(--amt == 0) break;
                    }
                    // 如果剩余病态层数为0，则退出循环
                    if(amt == 0) break;
                    // 累加减免，如果减免大于等于100，则退出循环
                    reduction += base.DynamicVars["DamageReductionPercent"].BaseValue;
                    if(reduction >= 100m) break;
                }
                // 如果剩余病态层数为0或减免大于等于100，则退出循环
                if(amt == 0 || reduction >= 100m) break;
            }
        }
        //$"病态伤害预估计算：结束计算伤害，攻击者{base.Owner.Name}，触发次数{num2}，攻击次数{atkTimes}，伤害{num}".LogInfo();
		return (int)num;
	}

    /// <summary>
    /// 施加者为玩家时，设置施加者名称
    /// </summary>
    /// <param name="applier"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
        // 如果施加者为空，则返回
        if(applier == null) return;
        // 如果施加者为玩家
        if(applier.IsPlayer){
            // 如果施加者没有痴迷对象，则给予痴迷对象
            if(!applier.HasPower<InfatuationTargetPower>())
            {
                await PowerCmd.Apply<InfatuationTargetPower>(applier, 1, applier, null);
            }
        }
	}
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 如果攻击目标或者攻击者为空，则返回
        if(target == null || dealer == null) return;
        // 如果攻击者不是病态持有者，则返回
        if(dealer != base.Owner) return;
        // 如果攻击目标为宠物，则不造成伤害
        if(target.IsPet) return;
        bool dealDamge = true;
        // 玩家持有时
        if(base.Owner.IsPlayer){
            // 如果卡牌来源为空，或者是自己攻击自己，则不造成伤害
            if(cardSource == null || target == base.Owner) dealDamge = false;
            // 如果伤害为0，则不造成伤害
            if(amount == 0) dealDamge = false;
        }else{
            // 敌人持有时，判断攻击目标是否有痴迷对象
           dealDamge = target.HasPower<InfatuationTargetPower>();
        }
        if(!dealDamge){
            return;
        }
        // 计算减免
        int mitigation = base.Owner.GetPowerAmount<MorbidMitigationPower>();
        // 如果减免大于等于100，则不造成伤害
        if(mitigation >= 100) return;
        // 计算攻击次数
        int atkTimes = Mathf.Min(Amount, 1 + base.Owner.GetPowerAmount<MadlyLovePower>());
        while(atkTimes > 0){
            // 造成伤害
            await CreatureCmd.Damage(new MorbidPlayerChoiceContext(), dealer, Amount * (100m - mitigation) / 100m, MORBID_VALUE_PROP, target);
            // 如果病态持有者死亡，则退出循环
            if(dealer.IsDead) return;
            // 减少一层病态
            await PowerCmd.Decrement(this);
            atkTimes--;
        }
        // 给予免疫
        await PowerCmd.Apply<MorbidMitigationPower>(dealer, base.DynamicVars["DamageReductionPercent"].BaseValue, null, null);
    }
    /// <summary>
    /// 死亡复活后不会移除
    /// </summary>
    /// <returns></returns>
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }

}
