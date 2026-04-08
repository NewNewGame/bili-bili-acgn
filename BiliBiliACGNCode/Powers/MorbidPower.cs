//****************** 代码文件申明 ***********************
//* 文件：MorbidPower(病态)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当你对敌方造成伤害时，自身受到病态层数的伤害。
//*******************************************************

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MorbidPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageReductionPercent", 25m),
        new StringVar("Applier"),
    ];
    /// <summary>
    /// 施加者为玩家时，设置施加者名称
    /// </summary>
    /// <param name="applier"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
        if(applier.IsPlayer){
            ((StringVar)base.DynamicVars["Applier"]).StringValue = PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, applier.Player.NetId);
        }
		return Task.CompletedTask;
	}
    /// <summary>
    /// 对敌方造成伤害时，自身受到病态层数的伤害
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="dealer"></param>
    /// <param name="result"></param>
    /// <param name="props"></param>
    /// <param name="target"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 如果伤害为0，则返回
        if(result.TotalDamage == 0) return;
        // 如果攻击目标或者攻击者为空，则返回
        if(target == null || dealer == null) return;
        bool dealDamge = true;
        // 玩家持有时
        if(base.Owner.IsPlayer){
            // 如果攻击者不是该玩家，则不造成伤害
            if(dealer != base.Owner) dealDamge = false;
            // 如果卡牌来源为空，则不造成伤害
            if(cardSource == null) dealDamge = false;
        }else{
            // 敌人持有时
            // 如果攻击目标不是施加者，则不造成伤害
            if(target != base.Applier) dealDamge = false;
        }
        if(!dealDamge) return;
        // 计算减免
        int mitigation = base.Owner.GetPowerAmount<MorbidMitigationPower>();
        // 如果减免大于等于100，则不造成伤害
        if(mitigation >= 100) return;
        // 计算攻击次数
        int atkTimes = Mathf.Min(Amount, 1 + target.GetPowerAmount<DistortionScholarPower>());
        while(atkTimes > 0){
            // 造成伤害
            await CreatureCmd.Damage(choiceContext, dealer, Amount * (100m - mitigation) / 100m, ValueProp.Unpowered, target);
            // 减少一层病态
            await PowerCmd.Decrement(this);
            atkTimes--;
        }
        // 给予免疫
        await PowerCmd.Apply<MorbidMitigationPower>(target, base.DynamicVars["DamageReductionPercent"].BaseValue, base.Owner, null);
    }
    /// <summary>
    /// 回合结束时减少一层病态
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Side)
		{
			await PowerCmd.Decrement(this);
		}
	}

}
