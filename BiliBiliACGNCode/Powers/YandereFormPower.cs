//****************** 代码文件申明 ***********************
//* 文件：YandereFormPower(病娇形态)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当你或女儿造成伤害时，同时给予等量的病态。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class YandereFormPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 如果攻击者为空，则返回
        if(dealer == null) return;

        // 如果攻击者是女儿或你自身，则施加病态
        if(dealer == base.Owner)
        {
            // 如果伤害来源为病态，则返回
            if(props == MorbidPower.MORBID_VALUE_PROP && cardSource == null) return;
            await PowerCmd.Apply<MorbidPower>(target, Amount * result.TotalDamage, dealer, cardSource);
        }else if(dealer.PetOwner != null && dealer.PetOwner == base.Owner.Player){
            await PowerCmd.Apply<MorbidPower>(target, Amount * result.TotalDamage, dealer.PetOwner.Creature, cardSource);
        }

    }

}
