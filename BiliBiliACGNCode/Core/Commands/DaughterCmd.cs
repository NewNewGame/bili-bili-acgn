//****************** 代码文件申明 ***********************
//* 文件：DaughterCmd
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：女儿命令辅助类
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Creatures;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Commands;

public static class DaughterCmd
{
    /// <summary>
    /// 召唤女儿
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static async Task<Creature?> SummonDaughter(this Creature creature)
    {
        var daughter = await PlayerCmd.AddPet<Itsuka>(creature.Player);
        if(daughter == null) return null;
        await PowerCmd.Apply<CombatForYouPower>(daughter, 1, daughter, null);
        return daughter;
    }
    /// <summary>
    /// 获取女儿
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static Creature? GetDaughter(this Creature creature)
    {
        var daughter = creature.Pets.FirstOrDefault(pet => pet.Monster is Itsuka);
        if(daughter == null){
            daughter = creature.CombatState?.Allies.FirstOrDefault((Creature c) => c.Monster is Itsuka && c.PetOwner?.Creature == creature);
        }
        return daughter;
    }
    /// <summary>
    /// 判断有没有女儿
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static bool HasDaughter(this Creature creature)
    {
        return creature.GetDaughter() != null;
    }
    /// <summary>
    /// 复活女儿
    /// </summary>
    /// <param name="daughter"></param>
    /// <returns></returns>
    public static Task ReviveDaughter(this Creature daughter)
    {
        var owner = daughter.PetOwner;
        if(owner == null) return Task.CompletedTask;
        owner.PlayerCombatState?.AddPetInternal(daughter);
        return Task.CompletedTask;
    }
    /// <summary>
    /// 女儿攻击指令
    /// </summary>
    public static async Task ApplyAttack(this Creature owner, decimal value, PlayerChoiceContext choiceContext, IEnumerable<Creature> targets)
    {
        var daughter = owner.GetDaughter();
        if(daughter == null) return;
        /* TODO：攻击特效
        foreach (Creature item in targets)
		{
			VfxCmd.PlayOnCreature(item, "vfx/vfx_attack_lightning");
		}
        */
		await CreatureCmd.Damage(choiceContext, targets, value, ValueProp.Move, daughter);
        await AfterDamageGiven(choiceContext, daughter);
    }
    /// <summary>
    /// 女儿攻击指令
    /// </summary>
    public static async Task<IEnumerable<DamageResult>> ApplyAttack(this Creature owner, decimal value, PlayerChoiceContext choiceContext, Creature target)
    {
        var daughter = owner.GetDaughter();
        if(daughter == null) return [];
        /* TODO：攻击特效
        foreach (Creature item in targets)
		{
			VfxCmd.PlayOnCreature(item, "vfx/vfx_attack_lightning");
		}
        */
		var results = await CreatureCmd.Damage(choiceContext, target, value, ValueProp.Move, daughter);
        await AfterDamageGiven(choiceContext, daughter);
        return results;
    }
    /// <summary>
    /// 女儿提升最大生命值指令
    /// </summary>
    public static async Task AddTempHp(this Creature creature, decimal value, PlayerChoiceContext choiceContext)
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return;
        await PowerCmd.Apply<AddMaxHpTempPower>(daughter, value, creature, null);
    }
    /// <summary>
    /// 女儿能力指令
    /// </summary>
    public static async Task ApplyPower<TPower>(this Creature creature, decimal value, CardModel? cardSource) where TPower : PowerModel
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return;
        await PowerCmd.Apply<TPower>(daughter, value, creature, cardSource);
    }
    /// <summary>
    /// 获取女儿的某个能力层数
    /// </summary>
    /// <typeparam name="TPower"></typeparam>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static int GetDaughterPowerAmount<TPower>(this Creature creature) where TPower : PowerModel
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return 0;
        return daughter.GetPowerAmount<TPower>();
    }
    /// <summary>
    /// 伤害后处理
    /// </summary>
    private static async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? daughter){
        if(daughter == null) return;
        var player = daughter.PetOwner;
        if(player == null) return;
        // 处理高高在上的抽卡逻辑
        int drawCards = player.Creature.GetPowerAmount<AloftThronePower>();
        if(drawCards > 0){
            // 如果不是在出牌阶段，则下回合抽牌
            if(RunManager.Instance.ActionQueueSynchronizer.CombatState != ActionSynchronizerCombatState.PlayPhase){
                await PowerCmd.Apply<DrawCardsNextTurnPower>(player.Creature, drawCards, player.Creature, null);
            }else{
                await CardPileCmd.Draw(choiceContext, drawCards, player);
            }
        }
        
    }

}