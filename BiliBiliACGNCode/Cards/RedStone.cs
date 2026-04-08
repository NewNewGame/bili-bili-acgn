//****************** 代码文件申明 ***********************
//* 文件：RedStone(赤石)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成17/23点伤害。只在在你的充能球大于等于3的时候才可以打出。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(17m, ValueProp.Move),
        new DynamicVar("OrbsRequired", 3m),
    ];

    public RedStone() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    public override bool CanPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 限制：仅当充能球数量 >= OrbsRequired 时才能打出
        return base.CanPlay(choiceContext, cardPlay);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 条件满足时，对所有敌人造成伤害
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
