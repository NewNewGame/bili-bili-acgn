//****************** 代码文件申明 ***********************
//* 文件：TasteHistorianPower(品史官)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你激发 1 个充能球时，抽 Amount 张牌
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TasteHistorianPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 每当你激发 1 个充能球时，抽 Amount 张牌
    /// </summary>
    /// <param name="choiceContext">选择上下文</param>
    /// <param name="orb">充能球</param>
    /// <param name="targets">目标</param>
    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if(orb.Owner != base.Owner.Player) return;
        await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
    }

}
