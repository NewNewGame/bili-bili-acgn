//****************** 代码文件申明 ***********************
//* 文件：MyConfession(我的忏悔)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：使敌人失去{Damage:diff()}点生命，重复等同于[gold]红温[/gold]次数的攻击，随后退出[gold]红怒[/gold]。消耗。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class MyConfession : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AngerPower>(),HoverTipFactory.FromPower<BerserkPower>()];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Unblockable|ValueProp.Unpowered|ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedTimes").WithMultiplier((card, creature) => card.Owner.Creature.GetPowerAmount<AngerPower>())
    ];

    public MyConfession() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target != null){
            // 按当前 AngerPower 层数重复造成 Damage 次攻击；最后移除 RagePower（退出红怒）
            int atkTimes = (int)((CalculatedVar)base.DynamicVars["CalculatedTimes"]).Calculate(cardPlay.Target);
            for(int i = 0; i < atkTimes; i++){
                await CreatureCmd.Damage(choiceContext, cardPlay.Target, base.DynamicVars.Damage, base.Owner.Creature, this);
                if(cardPlay.Target.IsDead) break;
            }
        }


        if(base.Owner.Creature.HasPower<BerserkPower>()){
            await PowerCmd.Remove<BerserkPower>(base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(2m);
    }
}
