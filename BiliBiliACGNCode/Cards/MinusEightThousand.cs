//****************** 代码文件申明 ***********************
//* 文件：MinusEightThousand(减8000)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：生成1/2个随机充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class MinusEightThousand : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("RandomOrbs", 1m),
    ];

    public MinusEightThousand() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 生成1/2个随机充能球
        int num = (int)base.DynamicVars["RandomOrbs"].BaseValue;
        if(base.IsUpgraded)++num;
        for(int i = 0; i < num; i++){
            await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(this),base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["RandomOrbs"].UpgradeValueBy(1m);
    }
}
