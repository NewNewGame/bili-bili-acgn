//****************** 代码文件申明 ***********************
//* 文件：AttackOrb
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：进击充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;

public sealed class AttackOrb : OrbBaseModel
{
	protected override string ChannelSfx => "event:/sfx/characters/defect/defect_glass_channel";

    public override Color DarkenedColor => new Color("008585");
    public override decimal PassiveVal => ModifyOrbValue(0m);
    public override decimal EvokeVal => ModifyOrbValue(5m);

	public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
	{
		await Passive(choiceContext, null);
	}

	public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
	{
		Trigger();
		await ApplyAttack(PassiveVal, target, choiceContext);
	}

	public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
	{
		return await ApplyAttack(EvokeVal, null, playerChoiceContext);
	}
   	private async Task<IEnumerable<Creature>> ApplyAttack(decimal value, Creature? target, PlayerChoiceContext choiceContext)
	{
		List<Creature> list = (from e in base.CombatState.GetOpponentsOf(base.Owner.Creature)
			where e.IsHittable
			select e).ToList();
		if (list.Count == 0)
		{
			return Array.Empty<Creature>();
		}
		bool allEnemies = base.Owner.Creature.HasPower<AnimeMasterPower>();
		IReadOnlyList<Creature> targets = (allEnemies) ? list : ((target == null) ? new List<Creature>(){base.Owner.RunState.Rng.CombatTargets.NextItem(list)} : new List<Creature>{target});

        await DaughterCmd.ApplyAttack(base.Owner.Creature, value, choiceContext, targets);
		return targets;
	}
}   