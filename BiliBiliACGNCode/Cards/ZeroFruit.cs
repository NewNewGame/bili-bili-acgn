//****************** 代码文件申明 ***********************
//* 文件：ZeroFruit(0果)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：女儿向敌人发动1次[gold]进攻[/gold]。给予{Weak:diff()}层[gold]虚弱[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class ZeroFruit : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Weak", 1m)
    ];

    public ZeroFruit() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 女儿向敌人发动1次[gold]进攻[/gold]。给予{Weak:diff()}层[gold]虚弱[/gold]。
        await DaughterCmd.ApplyAttack(base.Owner.Creature, 0m, choiceContext, cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(base.Owner.Creature, base.DynamicVars["Weak"].BaseValue, base.Owner.Creature, this);
    }
}
