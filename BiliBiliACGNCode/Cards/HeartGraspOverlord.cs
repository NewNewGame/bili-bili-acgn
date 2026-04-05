//****************** 代码文件申明 ***********************
//* 文件：HeartGraspOverlord(心脏掌握（骨王）)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：使敌人失去{Damage:diff()}点生命，并给予{Vulnerable:diff()}层易伤。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class HeartGraspOverlord : CardBaseModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("Vulnerable", 1m)
    ];

    public HeartGraspOverlord() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 直接生命移除/不可格挡伤害 + 易伤层数
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, base.DynamicVars["Damage"].BaseValue, base.DynamicVars.Damage.Props, this);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars["Vulnerable"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(7m);
        base.DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
