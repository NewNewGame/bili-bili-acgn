//****************** 代码文件申明 ***********************
//* 文件：ArchbishopPower(大主教)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：本回合女儿每次攻击指定敌人时，你获得 Amount 点力量（关联目标与攻击事件 TODO）
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

//****************** 代码文件申明 ***********************
//* 文件：ArchbishopPower(大主教)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：本回合女儿每次攻击指定敌人时，你获得 Amount 点力量
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ArchbishopPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if(target != base.Owner) return;
        if(dealer == null || dealer.Monster is not Itsuka) return;
        await PowerCmd.Apply<StrengthPower>(dealer, base.Amount, null, null);
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == CombatSide.Enemy){
            await PowerCmd.Remove(this);
        }
    }


}
