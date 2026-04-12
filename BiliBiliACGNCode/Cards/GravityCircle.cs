//****************** 代码文件申明 ***********************
//* 文件：GravityCircle(重力圈)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：若敌方拥有病态，则给予9/12层病态。
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
public sealed class GravityCircle : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    // 如果有敌人有病态，则发光
    protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any(e => e.HasPower<MorbidPower>()) ?? false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MorbidStacks", 9m),
    ];

    public GravityCircle() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 如果选取目标有病态，则给予9/12层病态
        if(cardPlay.Target?.HasPower<MorbidPower>() ?? false){
            await PowerCmd.Apply<MorbidPower>(cardPlay.Target, base.DynamicVars["MorbidStacks"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["MorbidStacks"].UpgradeValueBy(3m);
    }
}
