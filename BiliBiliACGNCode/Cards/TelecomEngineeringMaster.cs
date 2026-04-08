//****************** 代码文件申明 ***********************
//* 文件：TelecomEngineeringMaster(通信工程硕士)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：你每激发1个充能球，获得1点集中。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class TelecomEngineeringMaster : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke),HoverTipFactory.FromPower<FocusPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Focus", 1m),
    ];

    public TelecomEngineeringMaster() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加能力：每激发 1 个充能球，获得集中
        await PowerCmd.Apply<TelecomEngineeringMasterPower>(base.Owner.Creature, base.DynamicVars["Focus"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
