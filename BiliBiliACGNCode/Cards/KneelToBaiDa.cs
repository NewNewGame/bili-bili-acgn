//****************** 代码文件申明 ***********************
//* 文件：KneelToBaiDa(给百大跪下！)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：造成10点伤害，激发你最右侧的充能球两次。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class KneelToBaiDa : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
    ];

    public KneelToBaiDa() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        if(base.Owner.PlayerCombatState.OrbQueue.Orbs.Count > 0){
            await OrbCmd.EvokeNext(choiceContext, base.Owner, false);
            await OrbUtils.OrbEvokeWait();
            await OrbCmd.EvokeNext(choiceContext, base.Owner, true);
        }
    }

    protected override void OnUpgrade() { 
        base.DynamicVars["Damage"].UpgradeValueBy(5m);
    }
}
