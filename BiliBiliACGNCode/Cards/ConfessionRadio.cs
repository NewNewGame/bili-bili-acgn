//****************** 代码文件申明 ***********************
//* 文件：ConfessionRadio(忏悔电台)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：造成{Damage:diff()}点伤害，并获得等量于所造成伤害的[gold]格挡[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class ConfessionRadio : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
    ];

    public ConfessionRadio() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害，获得格挡
        var attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
		await CreatureCmd.GainBlock(base.Owner.Creature, attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage), ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(4m);
    }
}
