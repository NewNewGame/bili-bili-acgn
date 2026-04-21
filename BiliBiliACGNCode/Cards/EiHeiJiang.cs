//****************** 代码文件申明 ***********************
//* 文件：EiHeiJiang(诶嘿酱)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：随机给予敌人3层病态3/4次。
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
public sealed class EiHeiJiang : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MorbidPerHit", 3m),
        new DynamicVar("Hits", 3m),
    ];

    public EiHeiJiang() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.CombatState == null) return;
        int hitCount = (int)base.DynamicVars["Hits"].BaseValue;
        var enemies = base.CombatState.HittableEnemies;
        for(int i = 0; i < hitCount; i++){
            await PowerCmd.Apply<MorbidPower>(base.CombatState.RunState.Rng.CombatTargets.NextItem(enemies), base.DynamicVars["MorbidPerHit"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Hits"].UpgradeValueBy(1m);
    }
}
