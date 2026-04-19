//****************** 代码文件申明 ***********************
//* 文件：TasteHistorian(品史官)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：能力：每当你激发1个充能球时，抽1/2张牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class TasteHistorian : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Evoke),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    public TasteHistorian() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TasteHistorianPower>(base.Owner.Creature, base.DynamicVars["Cards"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 减1费用
        base.EnergyCost.UpgradeBy(-1);
    }
}
