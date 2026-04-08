//****************** 代码文件申明 ***********************
//* 文件：BigNose(大鼻子)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：对所有敌人造成11点伤害。激发所有充能球。
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
public sealed class BigNose : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move),
    ];

    public BigNose() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
        // 激发所有充能球
        int numberOfOrbs = base.Owner.PlayerCombatState?.OrbQueue.Orbs.Count ?? 0;
        for(int i = 0; i < numberOfOrbs; i++)
        {
            await OrbCmd.EvokeNext(choiceContext, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        // 降低1点耗能
        base.EnergyCost.UpgradeBy(-1);
    }
}
