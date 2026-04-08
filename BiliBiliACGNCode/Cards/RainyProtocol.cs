//****************** 代码文件申明 ***********************
//* 文件：RainyProtocol(雨色协议)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：给予所有盟友女儿拥有的力量相同层数，本回合后失去。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class RainyProtocol : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public RainyProtocol() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 如果没有女儿那就返回
        var daughter = base.Owner.Creature.GetDaughter();
        if(daughter == null) return;
        // 获得女儿拥有的力量层数
        int strength = daughter.GetPowerAmount<StrengthPower>();
        if(strength == 0) return;
        // 给予所有盟友女儿拥有的力量相同层数
        foreach(var teammate in base.CombatState.Players)
        {
            if(teammate == base.Owner || teammate.Creature.IsDead) continue;
            await PowerCmd.Apply<FlexPotionPower>(teammate.Creature, strength, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
