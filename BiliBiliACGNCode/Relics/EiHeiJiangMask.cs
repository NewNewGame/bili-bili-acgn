//* 文件：EiHeiJiangMask
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：欸嘿酱的面具 战斗开始时，召唤一果进行助战
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;


[Pool(typeof(SharedRelicPool))]
public sealed class EiHeiJiangMask : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("Power", 3), 
		new DynamicVar("AttackOrb", 1),
		new DynamicVar("Hp", 5m),
	];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<StrengthPower>(),
		HoverTipFactory.FromOrb<AttackOrb>()
	];

    public override async Task BeforeCombatStart()
	{
		await SummonPet();
	}
	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		// 回合开始时，召唤充能球
		if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
		{
			for (int i = 0; (decimal)i < base.DynamicVars["AttackOrb"].BaseValue; i++)
			{
				await OrbCmd.Channel<AttackOrb>(CombatUtils.GetTemporaryPlayerChoiceContext(), base.Owner);
			}
		}
	}
	private async Task SummonPet()
	{
		var daughter = await DaughterCmd.SummonDaughter(base.Owner.Creature);
		daughter.SetMaxHpInternal(base.DynamicVars["Hp"].BaseValue + 1);
		daughter.SetCurrentHpInternal(base.DynamicVars["Hp"].BaseValue + 1);
		await DaughterCmd.ApplyPower<StrengthPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, null);
	}

}
