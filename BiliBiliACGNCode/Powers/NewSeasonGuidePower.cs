//****************** 代码文件申明 ***********************
//* 文件：NewSeasonGuidePower(新番导视)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：回合开始时触发最左侧充能球被动 Amount 次
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonGuidePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 回合开始取队列最后一个球触发被动 OrbCmd.Passive 循环 Amount 次
    /// </summary>
    /// <param name="choiceContext">选择上下文</param>
    /// <param name="player">玩家</param>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner.Player && player.PlayerCombatState.OrbQueue.Orbs.Count != 0)
		{
            var orb = player.PlayerCombatState.OrbQueue.Orbs.Last();
			for (int i = 0; i < base.Amount; i++)
			{
				await OrbCmd.Passive(choiceContext, orb, null);
				await Cmd.Wait(0.25f);
			}
		}
    }
}
