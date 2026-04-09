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
        return creature.Pets.FirstOrDefault(pet => pet.Monster is Itsuka);
    }
    /// <summary>
    /// 判断有没有女儿
    /// </summary>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static bool HasDaughter(this Creature creature)
    {
        return creature.Pets.Any(pet => pet.Monster is Itsuka);
    }
    /// <summary>
    /// 女儿攻击指令
    /// </summary>
    public static async Task ApplyAttack(this Creature creature, decimal value, PlayerChoiceContext choiceContext, IEnumerable<Creature> targets)
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return;
        /* TODO：攻击特效
        foreach (Creature item in targets)
		{
			VfxCmd.PlayOnCreature(item, "vfx/vfx_attack_lightning");
		}
        */
		await CreatureCmd.Damage(choiceContext, targets, value, ValueProp.Move, daughter);
    }
    /// <summary>
    /// 女儿格挡指令
    /// </summary>
    public static async Task ApplyBlock(this Creature creature, decimal value, PlayerChoiceContext choiceContext)
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return;
        await CreatureCmd.GainBlock(daughter, value, ValueProp.Unpowered, null);
    }
    /// <summary>
    /// 女儿能力指令
    /// </summary>
    public static async Task ApplyPower<TPower>(this Creature creature, decimal value, PlayerChoiceContext choiceContext, CardModel? cardSource) where TPower : PowerModel
    {
        var daughter = creature.GetDaughter();
        if(daughter == null) return;
        await PowerCmd.Apply<TPower>(daughter, value, creature, cardSource);
    }
}