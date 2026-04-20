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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MawaruMawaruPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsInstanced => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Applier")];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if(applier != null && applier.IsPlayer){
            ((StringVar)base.DynamicVars["Applier"]).StringValue = 
            PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, base.Applier.Player.NetId);
        }
        return base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 当 base.Owner 死亡时，对其它敌人造成等同于 base.Owner 最大生命值的伤害。
        if(target != base.Owner) return;

        // 对其它敌人造成等同于 base.Owner 最大生命值的伤害。
        var enemies = base.Owner.CombatState.Enemies.ToList();
        foreach(var enemy in enemies){
            if(enemy == base.Owner || enemy.IsDead) continue;
            await CreatureCmd.Damage(choiceContext, enemy, base.Owner.MaxHp, ValueProp.Unpowered, base.Applier);
        }
    }
}
