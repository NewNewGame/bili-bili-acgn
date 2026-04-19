//****************** 代码文件申明 ***********************
//* 文件：NewYearGalaDeathSong(拜年祭死歌)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：对一个敌人造成{Damage:diff()}点伤害，如果处于[gold]红怒[/gold]，则额外造成{ExtraDamage:diff()}点伤害。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NewYearGalaDeathSong : CardBaseModel
{
    #region 卡牌关键词与悬停
    // 红怒状态悬停提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BerserkPower>()];
    // 红怒状态发光
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.HasPower<BerserkPower>();
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(16m),
        new ExtraDamageVar(5m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.Creature.HasPower<BerserkPower>() ? 1 : 0)
    ];

    public NewYearGalaDeathSong() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对目标敌人造成伤害
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(1m);
        base.DynamicVars["ExtraDamage"].UpgradeValueBy(2m);
    }
}
