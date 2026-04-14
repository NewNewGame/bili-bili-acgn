//****************** 代码文件申明 ***********************
//* 文件：NobleLadyPower(大家闺秀)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：本回合每激发1个充能球女儿向该敌人进攻1次。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NobleLadyPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke), HoverTipFactory.Static(CustomeHoverTips.AttackOrb)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Applier")];
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		((StringVar)base.DynamicVars["Applier"]).StringValue = PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, base.Applier.Player.NetId);
		return Task.CompletedTask;
	}
    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if(orb.Owner != base.Applier?.Player) return;
        await DaughterCmd.ApplyAttack(base.Applier, 0m, choiceContext, base.Owner);
    }
}
