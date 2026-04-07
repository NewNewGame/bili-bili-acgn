//****************** 代码文件申明 ***********************
//* 文件：AnimeSwordPower
//* 作者：wheat
//* 创建时间：2026/04/03 18:24:00 星期五
//* 描述：能力 动漫区的剑 每当你获得[gold]红温值[/gold]，对随机敌人造成{Damage:diff()}点伤害。
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeSwordPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is AngerPower && amount > 0 && applier == base.Owner){
            var enemies = base.CombatState.HittableEnemies;
            var enemy = base.CombatState.RunState.Rng.CombatTargets.NextItem(enemies);
            if(enemy != null){
                await CreatureCmd.Damage(CombatUtils.GetTemporaryPlayerChoiceContext(), enemy, base.Amount, ValueProp.Unpowered, base.Owner, null);
            }
        }
    }
}
