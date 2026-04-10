//****************** 代码文件申明 ***********************
//* 文件：DistortionScholar(扭曲学家)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：给予所有敌人1/2层狂恋。病态会额外触发1/2次。
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
public sealed class DistortionScholar : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>(), HoverTipFactory.FromPower<MadlyLovePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ExtraTriggers", 1m),
    ];

    public DistortionScholar() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.CombatState != null){
        // 施加能力：病态额外触发次数
            foreach(var enemy in base.CombatState.HittableEnemies){
                await PowerCmd.Apply<MadlyLovePower>(enemy, base.DynamicVars["ExtraTriggers"].BaseValue, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ExtraTriggers"].UpgradeValueBy(1m);
    }
}
