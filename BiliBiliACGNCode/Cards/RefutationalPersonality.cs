//****************** 代码文件申明 ***********************
//* 文件：RefutationalPersonality(反驳形人格)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：获得{Thorns:diff()}层[gold]荆棘[/gold]；[gold]红怒[/gold]时改为{ThornsRage:diff()}层。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class RefutationalPersonality : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>(),
        HoverTipFactory.FromPower<AngerPower>()
    ];
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<AngerChargePower>() +base.DynamicVars["Anger"].BaseValue >= AngerChargePower.MAXCHARGE;

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Thorns", 1m),
        new DynamicVar("Anger", 5m)
    ];

    public RefutationalPersonality() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 添加荆棘BUFF
        await PowerCmd.Apply<ThornsPower>(base.Owner.Creature, base.DynamicVars["Thorns"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<AngerPower>(base.Owner.Creature, base.DynamicVars["Anger"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Anger"].UpgradeValueBy(1m);
        base.AddKeyword(CardKeyword.Retain);
    }
}
