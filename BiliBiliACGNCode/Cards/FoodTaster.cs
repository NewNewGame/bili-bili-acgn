//****************** 代码文件申明 ***********************
//* 文件：FoodTaster(试吃员)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：抽牌堆每有4/3张牌，获得1点能量。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class FoodTaster : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DeckPer", 4m),
        new EnergyVar(1),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedEnergy").WithMultiplier((card, _) => {
            return (int)(PileType.Draw.GetPile(card.Owner).Cards.Count() / card.DynamicVars["DeckPer"].BaseValue);
        })
    ];

    public FoodTaster() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(((CalculatedVar)base.DynamicVars["CalculatedEnergy"]).Calculate(base.Owner.Creature), base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DeckPer"].UpgradeValueBy(-1m);
    }
}
