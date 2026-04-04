//****************** 代码文件申明 ***********************
//* 文件：CorrectTeamPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 正确车队
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class CorrectTeamPower : PowerBaseModel
{
    protected override string customIconPath => "correctteam";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 修改能量上限
    /// </summary>
    /// <param name="player"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != base.Owner.Player)
		{
			return amount;
		}
		return amount + (decimal)base.Amount/2m;
	}

    // 红怒状态下额外抽 Amount 张牌
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is BerserkPower && amount > 0 && applier == base.Owner){
            // 抽牌
            await CardPileCmd.Draw(CombatHelper.GetTemporaryPlayerChoiceContext(), base.Amount, base.Owner.Player);
        }
    }


}
