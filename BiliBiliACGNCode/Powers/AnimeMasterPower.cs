//****************** 代码文件申明 ***********************
//* 文件：AnimeMasterPower(动漫高手)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当你激发充能球时，给予女儿力量。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeMasterPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke), HoverTipFactory.FromPower<StrengthPower>()];

    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if(orb.Owner.Creature != base.Owner) return;
        var daughter = base.Owner.GetPet<Itsuka>();
        if(daughter == null) return;
        // 给予女儿力量
        await PowerCmd.Apply<StrengthPower>(daughter, Amount, base.Owner, null);
    }

}
