//****************** 代码文件申明 ***********************
//* 文件：ImSoDespairPower(我好绝望啊)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：本回合，拥有病态的敌人对你造成伤害减半。
//*******************************************************

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ImSoDespairPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 本回合，拥有病态的敌人对你造成伤害减半
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amount"></param>
    /// <param name="props"></param>
    /// <param name="dealer"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (dealer == null || !dealer.HasPower<MorbidPower>())
		{
			return 1m;
		}

		return 0.5m;
	}

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // 回合结束时移除
        if(side == CombatSide.Enemy)
        {
            await PowerCmd.Decrement(this);
        }
    }
}
