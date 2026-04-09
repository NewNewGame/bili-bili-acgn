//****************** 代码文件申明 ***********************
//* 文件：InfatuationTargetPower
//* 作者：wheat
//* 创建时间：2026/04/09
//* 描述：痴迷对象
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class InfatuationTargetPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>
    /// 死亡时转让给下一个目标
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="creature"></param>
    /// <param name="wasRemovalPrevented"></param>
    /// <param name="deathAnimLength"></param>
    /// <returns></returns>
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 如果死亡被阻止，则不转让
        if(wasRemovalPrevented) return;
        // 如果只有1个玩家，则不转让
        if(base.CombatState.Players.Count == 1) return;
        // 获取下一个目标
        var nextTarget = base.CombatState.Players.FirstOrDefault(p => p.Creature != creature && !p.Creature.HasPower<InfatuationTargetPower>() && p.Creature.IsAlive);
        // 如果下一个目标不为空，则转让给下一个目标
        if(nextTarget != null){
            await PowerCmd.Apply<InfatuationTargetPower>(nextTarget.Creature, 1, nextTarget.Creature, null);
        }
    }
}