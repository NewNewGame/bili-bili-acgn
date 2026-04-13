//****************** 代码文件申明 ***********************
//* 文件：WanCeJin(万策尽)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：激发你所有的充能球，每个充能球获得1/2点集中。消耗。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class WanCeJin : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromPower<FocusPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("FocusPerOrb", 1m),
    ];

    public WanCeJin() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int cnt = base.Owner.PlayerCombatState.OrbQueue.Orbs.Count;
        for(int i = 0; i < cnt; i++){
            await OrbCmd.EvokeNext(choiceContext, base.Owner);
        }
        await PlayerCmd.GainEnergy(base.DynamicVars["FocusPerOrb"].BaseValue * cnt, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FocusPerOrb"].UpgradeValueBy(1m);
    }
}
