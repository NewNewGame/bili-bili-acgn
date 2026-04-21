//****************** 代码文件申明 ***********************
//* 文件：LifeOrb
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：生命充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;

public sealed class LifeOrb : OrbBaseModel
{
	protected override string ChannelSfx => "event:/sfx/characters/defect/defect_frost_channel";

	public override Color DarkenedColor => new Color("7860a7");

	public override decimal PassiveVal => ModifyOrbValue(2m);

	public override decimal EvokeVal => ModifyOrbValue(4m);

	public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
	{
		await Passive(choiceContext, null);
	}

	public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
	{
		if (target != null)
		{
			throw new InvalidOperationException("Block orbs cannot target creatures.");
		}
		Trigger();
		await DaughterCmd.AddTempHp(base.Owner.Creature, PassiveVal, choiceContext);
	}

	public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
	{
		PlayEvokeSfx();
		await DaughterCmd.AddTempHp(base.Owner.Creature, EvokeVal, playerChoiceContext);
		return new List<Creature>(){base.Owner.Creature};
	}
    
}   