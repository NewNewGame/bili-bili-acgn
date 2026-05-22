//****************** 代码文件申明 ***********************
//* 文件：AtFieldGenerator
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：A.T.力场发生器 每回合获得5点格挡，但每次受到未被格挡的伤害时，力场消失一回合。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class AtFieldGenerator : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 5m), new DynamicVar("Turns",2m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];
    private int _cooldown;
    private int Cooldown
    {
        get
        {
            return _cooldown;
        }
        set
        {
            AssertMutable();
            _cooldown = value;
            UpdateDisplay();
        }
    }
    private void UpdateDisplay()
	{
		base.Status = Cooldown == 0 ? RelicStatus.Normal : RelicStatus.Disabled;
	}
    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if(Cooldown > 0) return Task.CompletedTask;
        if(result.UnblockedDamage <= 0) return Task.CompletedTask;
        if (target == base.Owner.Creature && dealer != null && dealer != base.Owner.Creature)
		{
            Cooldown = base.DynamicVars["Turns"].IntValue;
		}
        return Task.CompletedTask;
    }
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if(side == base.Owner.Creature.Side){
            if(Cooldown > 0){
                --Cooldown;
            }
        }
        return Task.CompletedTask;
    }
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Creature.Side)
		{
            // 如果力场正在激活，给予玩家护盾
            if(Cooldown == 0)
            {
                Flash();
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, ValueProp.Unpowered, null);
            }
		}
    }
    public override Task AfterCombatEnd(CombatRoom _)
    {
        Cooldown = 0;
        return Task.CompletedTask;
    }

}
