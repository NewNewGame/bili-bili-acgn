//****************** 代码文件申明 ***********************
//* 文件：MawaruMawaruPower(马瓦鲁马瓦鲁)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：当这名敌人死亡时，对其他敌人造成等同于它最大生命值的伤害。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MawaruMawaruPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 当 base.Owner 死亡时，对其它敌人造成等同于 base.Owner 最大生命值的伤害。
        if(target != base.Owner) return;

        // 对其它敌人造成等同于 base.Owner 最大生命值的伤害。
        foreach(var enemy in base.Owner.CombatState.Enemies){
            if(enemy == base.Owner) continue;
            await CreatureCmd.Damage(choiceContext, enemy, base.Owner.MaxHp * Amount, ValueProp.Unpowered, base.Applier);
        }
    }
}
