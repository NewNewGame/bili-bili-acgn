//****************** 代码文件申明 ***********************
//* 文件：DeathRebirthCycle(死亡轮回)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：死亡后复活一次，复活后回复60%的生命值。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class DeathRebirthCycle : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    private bool _wasUsed;

	public override bool IsUsedUp => _wasUsed;
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HealAmount", 60m)];

	[SavedProperty]
	public bool BILIBILIACGN_DRC_WasUsed
	{
		get
		{
			return _wasUsed;
		}
		set
		{
			AssertMutable();
			_wasUsed = value;
			if (IsUsedUp)
			{
				base.Status = RelicStatus.Disabled;
			}
		}
	}

	public override bool ShouldDieLate(Creature creature)
	{
		if (creature != base.Owner.Creature)
		{
			return true;
		}
		if (BILIBILIACGN_DRC_WasUsed)
		{
			return true;
		}
		return false;
	}

	public override async Task AfterPreventingDeath(Creature creature)
	{
		Flash();
		BILIBILIACGN_DRC_WasUsed = true;
		decimal amount = Math.Max(1m, creature.MaxHp * base.DynamicVars["HealAmount"].BaseValue / 100m);
		await CreatureCmd.Heal(creature, amount);
	}
}
