//****************** 代码文件申明 ***********************
//* 文件：NtrGodfatherPower(NTR教父)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：在你的回合开始时，给予自身病态。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NtrGodfatherPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];


    /// <summary>
    /// Amount：回合开始时给予自身的病态层数
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner.Player)
        {
            await PowerCmd.Apply<MorbidPower>(base.Owner, Amount, base.Owner, null);
        }
    }
}
