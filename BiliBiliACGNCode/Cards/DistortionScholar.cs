//****************** 代码文件申明 ***********************
//* 文件：DistortionScholar(扭曲学家)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：病态会额外触发1/2次。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class DistortionScholar : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ExtraTriggers", 1m),
    ];

    public DistortionScholar() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 施加能力：病态额外触发次数
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ExtraTriggers"].UpgradeValueBy(1m);
    }
}
