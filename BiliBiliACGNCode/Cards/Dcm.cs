//****************** 代码文件申明 ***********************
//* 文件：Dcm(DCM)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：获得10/12点格挡，生成{StrengthOrb:diff()}个力量充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Dcm : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block),HoverTipFactory.Static(StaticHoverTip.Channeling),HoverTipFactory.FromOrb<StrengthOrb>(),];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("StrengthOrb", 1m)
    ];

    public Dcm() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得10/12点格挡，生成{StrengthOrb:diff()}个力量充能球
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, base.DynamicVars.Block.Props, cardPlay);
        int cnt = (int)base.DynamicVars["StrengthOrb"].BaseValue;
        for(int i = 0; i < cnt; i++){
            await OrbCmd.Channel<StrengthOrb>(choiceContext, base.Owner);
            if(i < cnt - 1){
                await OrbUtils.OrbChannelingWait();
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars["StrengthOrb"].UpgradeValueBy(1m);
    }
}
