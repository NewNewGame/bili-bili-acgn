//****************** 代码文件申明 ***********************
//* 文件：BrainWife(脑婆)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成26/34点伤害，给予自身3点病态。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class BrainWife : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(26m, ValueProp.Move),
        new DynamicVar("MorbidSelf", 3m),
    ];

    public BrainWife() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成伤害；并给予自身病态
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
        await PowerCmd.Apply<MorbidPower>(base.Owner.Creature, base.DynamicVars["MorbidSelf"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(7m);
    }
}
