//****************** 代码文件申明 ***********************
//* 文件：BlowWithThemAll(跟他爆了)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗3点[gold]红温[/gold]，对所有敌人造成{Damage:diff()}点伤害。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BlowWithThemAll : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AngerPower>()];
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<AngerPower>() >= base.DynamicVars["Anger"].BaseValue;
    protected override bool IsPlayable => base.Owner.Creature.GetPowerAmount<AngerPower>() >= base.DynamicVars["Anger"].BaseValue;

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Anger", 2m),
        new CalculationBaseVar(9m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.Creature.HasPower<BerserkPower>() ? 1 : 0)
    ];

    public BlowWithThemAll() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 爆炸效果
        IReadOnlyList<Creature> targets = base.CombatState.HittableEnemies;
		foreach (Creature item in targets)
		{
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(item));
		}
		await Cmd.CustomScaledWait(0.2f, 0.3f);
        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).FromCard(this).TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
        // 消耗3点红温
        await PowerCmd.Apply<AngerPower>(base.Owner.Creature, -base.DynamicVars["Anger"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(4m);
    }
}
