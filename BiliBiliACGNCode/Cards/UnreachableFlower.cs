//****************** 代码文件申明 ***********************
//* 文件：UnreachableFlower(高岭之花)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：给予所有敌人2层易伤，女儿向所有敌人各进攻1次。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class UnreachableFlower : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Vulnerable", 2m),
    ];

    public UnreachableFlower() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.CombatState == null) return;
        // 给予所有敌人2层易伤
        foreach(var enemy in base.CombatState.HittableEnemies){
            await PowerCmd.Apply<VulnerablePower>(enemy, base.DynamicVars["Vulnerable"].BaseValue, base.Owner.Creature, this);
        }
        // 女儿向所有敌人各进攻1次
        await DaughterCmd.ApplyAttack(base.Owner.Creature, 0m, choiceContext, base.CombatState.HittableEnemies);
    }

    protected override void OnUpgrade() { }
}
