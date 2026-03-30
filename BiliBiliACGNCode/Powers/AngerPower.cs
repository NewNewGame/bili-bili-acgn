//****************** 代码文件申明 ***********************
//* 文件：AngerPower
//* 作者：wheat
//* 创建时间：2026/03/29 星期日
//* 描述：能力 红温
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AngerPower : PowerBaseModel
{
    protected override string customIconPath => "anger";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Owner != dealer || cardSource == null)
		{
			return 0m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 0m;
		}
        if(cardSource.Keywords.Contains(CustomKeyWords.Anger))
        {
            return base.Amount;
        }
        return 0m;
    }

}
