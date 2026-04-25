//****************** 代码文件申明 ***********************
//* 文件：UnreachableFlower(高岭之花)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：造成{Damage:diff()}点伤害。抽{Cards:diff()}张牌。下回合抽{Cards:diff()}张牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class UnreachableFlower : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new CardsVar(1),
    ];

    public UnreachableFlower() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        // 抽1张牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        // 下回合抽1张牌
        await PowerCmd.Apply<DrawCardsNextTurnPower>(base.Owner.Creature, base.DynamicVars.Cards.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade() { 
        base.DynamicVars["Damage"].UpgradeValueBy(4m);
    }
}
