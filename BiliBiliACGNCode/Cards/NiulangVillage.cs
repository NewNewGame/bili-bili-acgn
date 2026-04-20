//****************** 代码文件申明 ***********************
//* 文件：NiulangVillage(牛郎村)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：X费。造成等同于[gold]红温值[/gold]×本次支付能量倍数的伤害；[gold]红怒[/gold]时倍率翻倍。消耗，保留。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NiulangVillage : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AngerPower>(),
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

    public NiulangVillage() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => {
            return card.Owner.Creature.GetPowerAmount<AngerPower>();
        }),
    ];


    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = base.ResolveEnergyXValue();
        if(base.IsUpgraded) x++;
        int anger = (int)((CalculatedDamageVar)base.DynamicVars["CalculatedDamage"]).Calculate(cardPlay.Target);
        await DamageCmd.Attack(anger * x)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

}
