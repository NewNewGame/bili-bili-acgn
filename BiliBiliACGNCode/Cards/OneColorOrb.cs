//****************** 代码文件申明 ***********************
//* 文件：OneColorOrb(一色股)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：生成1个力量充能球，触发所有力量充能球的被动1/2次。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class OneColorOrb : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromOrb<StrengthOrb>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("PassiveTriggers", 1m),
    ];

    public OneColorOrb() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OrbCmd.Channel<StrengthOrb>(choiceContext, base.Owner);
        int num = (int)base.DynamicVars["PassiveTriggers"].BaseValue;
        List<StrengthOrb> list = base.Owner.PlayerCombatState.OrbQueue.Orbs.OfType<StrengthOrb>().ToList();
        foreach (var item in list)
        {
            for (int i = 0; i < num; i++)
            {
                await OrbCmd.Passive(choiceContext, item, cardPlay.Target);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PassiveTriggers"].UpgradeValueBy(1m);
    }
}
