//****************** 代码文件申明 ***********************
//* 文件：AngerDelayPower
//* 作者：wheat
//* 创建时间：2026/04/04 星期六
//* 描述：能力 红温延迟
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AngerDelayPower : PowerBaseModel
{
    protected override string customIconPath => "anger_delay";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 如果当前回合是玩家回合，则施加红温，然后移除自身
        if(player == base.Owner.Player){
            await PowerCmd.Apply<AngerPower>(base.Owner, base.Amount, base.Owner, null);
            await PowerCmd.Remove<AngerDelayPower>(base.Owner);
        }
    }

}
