//****************** 代码文件申明 ***********************
//* 文件：ChuunibyouNote(中二笔记)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：给予10层病态。每有10层病态，额外给予3层病态。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class ChuunibyouNote : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(10m),
        new CalculationExtraVar(3m),
        new CalculatedVar("CalculatedMorbid").WithMultiplier((_, target) => 
        target?.GetPowerAmount<MorbidPower>()/10m ?? 0m)
    ];

    public ChuunibyouNote() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 施加能力：给予10层病态。每有10层病态，额外给予3层病态。
        await PowerCmd.Apply<MorbidPower>(cardPlay.Target, ((CalculatedVar)base.DynamicVars["CalculatedMorbid"]).Calculate(cardPlay.Target), base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CalculationBase"].UpgradeValueBy(2m);
        base.DynamicVars["CalculationExtra"].UpgradeValueBy(1m);
    }
}
