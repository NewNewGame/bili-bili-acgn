//****************** 代码文件申明 ***********************
//* 文件：CoinOperation(铸币操作)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：造成{Damage:diff()}点伤害。给予敌人{Tang:diff()}层[gold]变唐[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class CoinOperation : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GetTangPower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new DynamicVar("Tang", 2m)
    ];

    public CoinOperation() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害，给予敌人变唐
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<GetTangPower>(cardPlay.Target, base.DynamicVars["Tang"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(4m);
        base.DynamicVars["Tang"].UpgradeValueBy(1m);
    }
}
