//****************** 代码文件申明 ***********************
//* 文件：WhirlwindRangerCloak(旋风游侠披风)
//* 作者：wheat
//* 创建时间：2026/04/27 09:38:00 星期一
//* 描述：回合结束时，对所有敌人造成未用完的能量点数伤害。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(FunShikiRelicPool))]
public sealed class WhirlwindRangerCloak : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var combatState = base.Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        int energyLeft = base.Owner.PlayerCombatState.Energy;
        if (energyLeft <= 0)
        {
            return;
        }

        Flash();
        foreach (var enemy in combatState.HittableEnemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, energyLeft, ValueProp.Unpowered, base.Owner.Creature);
        }
    }

}

