//****************** 代码文件申明 ***********************
//* 文件：StrengthOrb
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：力量充能球
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;

public sealed class StrengthOrb : OrbBaseModel
{
    private decimal _evokeVal = 3m;
	protected override string ChannelSfx => "event:/sfx/characters/defect/defect_dark_channel";
	public override Color DarkenedColor => new Color("9001d3");

	public override decimal PassiveVal => ModifyOrbValue(5m);

	public override decimal EvokeVal => _evokeVal;

	public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
	{
		await Passive(choiceContext, null);
	}

	public override Task Passive(PlayerChoiceContext choiceContext, Creature? target)
	{
		if (target != null)
		{
			throw new InvalidOperationException("Strength orbs cannot target creatures.");
		}
		Trigger();
		_evokeVal += PassiveVal;
		NCombatRoom.Instance?.GetCreatureNode(base.Owner.Creature)?.OrbManager?.UpdateVisuals(OrbEvokeType.None);
		return Task.CompletedTask;
	}

	public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
	{
		await DaughterCmd.ApplyPower<StrengthPower>(base.Owner.Creature, (int)EvokeVal/3m, null);
		return new List<Creature>(){base.Owner.Creature};
	}
}   