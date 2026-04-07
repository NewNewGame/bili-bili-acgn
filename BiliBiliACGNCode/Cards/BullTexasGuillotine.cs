//****************** 代码文件申明 ***********************
//* 文件：BullTexasGuillotine(牛克萨斯断头台)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：X费。造成等同于[gold]红温值[/gold]×本次支付能量倍数的伤害；[gold]红怒[/gold]时倍率翻倍。消耗，保留。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BottleRagePower = BiliBiliACGN.BiliBiliACGNCode.Powers.BerserkPower;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BullTexasGuillotine : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AngerPower>(),
        HoverTipFactory.FromPower<BottleRagePower>()
    ];

    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = -1;
    protected override bool HasEnergyCostX => true;

    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public BullTexasGuillotine() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

        /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
		new ExtraDamageVar(1m),
		new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => {
            // 伤害 = 红温层数 × X(+1) ×（有 红温 则 2倍）
            int x = card.ResolveEnergyXValue();
            if(x == 0) return card.Owner.Creature.GetPowerAmount<AngerPower>();
            decimal num = x;
            if(card.IsUpgraded) ++num;
            decimal dmg = card.Owner.Creature.GetPowerAmount<AngerPower>() * num;
            return dmg;
        })
    ];


    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.AddKeyword(CardKeyword.Retain);
    }
}
