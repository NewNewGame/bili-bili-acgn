//****************** 代码文件申明 ***********************
//* 文件：MorbidPlayerChoiceContext
//* 作者：wheat
//* 创建时间：2026/04/22
//* 描述：病态玩家选择上下文
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.GameActions.Multiplayer;

public sealed class MorbidPlayerChoiceContext : PlayerChoiceContext
{
    public override ulong? OwnerId => null;

    public override Task SignalPlayerChoiceBegun(Player chooser, PlayerChoiceOptions options)
    {
        throw new NotImplementedException();
    }

    public override Task SignalPlayerChoiceEnded()
    {
        throw new NotImplementedException();
    }
}