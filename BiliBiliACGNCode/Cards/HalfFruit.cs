//****************** 代码文件申明 ***********************
//* 文件：HalfFruit(0.5果)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当女儿攻击敌人时，给予敌人{StrengthLoss:diff()}点变唐。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class HalfFruit : CardBaseModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GetTangPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Tang", 2m),
    ];

    public HalfFruit() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加能力：女儿攻击敌人时，给予敌人本回合变唐
        await DaughterCmd.ApplyPower<HalfFruitPower>(base.Owner.Creature, base.DynamicVars["Tang"].BaseValue, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
