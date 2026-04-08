//****************** 代码文件申明 ***********************
//* 文件：RedStone(赤石)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成17/23点伤害。只在在你的充能球大于等于3的时候才可以打出。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class RedStone : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override bool IsPlayable => base.Owner.PlayerCombatState?.OrbQueue.Orbs.Count() >= base.DynamicVars["OrbsRequired"].BaseValue;


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(17m, ValueProp.Move),
        new DynamicVar("OrbsRequired", 3m),
    ];

    public RedStone() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
