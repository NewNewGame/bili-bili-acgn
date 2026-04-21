//****************** 代码文件申明 ***********************
//* 文件：AttackOrb
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：进击充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;

public sealed class AttackOrb : OrbBaseModel
{
	protected override string ChannelSfx => "event:/sfx/characters/defect/defect_plasma_channel";
	protected override string EvokeSfx => "event:/sfx/characters/defect/defect_plasma_evoke";
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_plasma_passive";

    public override Color DarkenedColor => new Color("008585");
    public override decimal PassiveVal => ModifyOrbValue(0m);

    public override decimal EvokeVal => ModifyOrbValue(3m);

	public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
	{
		await Passive(choiceContext, null);
	}

	public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
	{
		Trigger();
		await ApplyAttack(PassiveVal, target, choiceContext, false);
	}

	public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
	{
		return await ApplyAttack(EvokeVal, null, playerChoiceContext, true);
	}
   	private async Task<IEnumerable<Creature>> ApplyAttack(decimal value, Creature? target, PlayerChoiceContext choiceContext, bool allEnemies)
	{
		List<Creature> list = (from e in base.CombatState.GetOpponentsOf(base.Owner.Creature)
			where e.IsHittable
			select e).ToList();
		if (list.Count == 0)
		{
			return Array.Empty<Creature>();
		}
		IReadOnlyList<Creature> targets = (allEnemies) ? list : ((target == null) ? new List<Creature>(){base.Owner.RunState.Rng.CombatTargets.NextItem(list)} : new List<Creature>{target});

		PlayEvokeSfx();
        await DaughterCmd.ApplyAttack(base.Owner.Creature, value, choiceContext, targets);
		return targets;
	}
}   