//****************** 代码文件申明 ***********************
//* 文件：SmNiuGe(sm牛哥)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗。将你的[gold]唐氏[/gold]层数翻倍。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class SmNiuGe : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TangShiPower>()];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public SmNiuGe() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 将你的[gold]唐氏[/gold]层数翻倍
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, base.Owner.Creature.GetPower<TangShiPower>()?.Amount ?? 0m, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
