//****************** 代码文件申明 ***********************
//* 文件：InfiniteBullness(无量牛处)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：造成{Damage:diff()}点伤害，获得{Power:diff()}点唐氏。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class InfiniteBullness : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TangShiPower>()];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CustomKeyWords.YYSY, CardKeyword.Innate, CardKeyword.Exhaust];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move),
        new DynamicVar("Power", 3m)
    ];

    public InfiniteBullness() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
        .FromCard(this)
        .Targeting(cardPlay.Target)
        .Execute(choiceContext);
        await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(6m);
        base.DynamicVars["Power"].UpgradeValueBy(2m);
    }
}
