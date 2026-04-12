//****************** 代码文件申明 ***********************
//* 文件：AhShortHaircutPower(哎短发)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：回合开始时给予所有敌人 Amount 层病态
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AhShortHaircutPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];

    // 在你的回合开始时，给予所有敌人 Amount 层病态
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == null || player != base.Owner.Player || player.Creature.CombatState == null) return;
        foreach(var enemy in player.Creature.CombatState.HittableEnemies){
            await PowerCmd.Apply<MorbidPower>(enemy, base.Amount, base.Owner, null);
        }
    }

}
