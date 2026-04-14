//****************** 代码文件申明 ***********************
//* 文件：BlackRabbit(黑兔)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：女儿向1名敌人进攻1次，并给予自身等同于造成伤害的格挡。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class BlackRabbit : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.Static(CustomeHoverTips.AttackOrb)];

    public BlackRabbit() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var damageResults = await DaughterCmd.ApplyAttack(base.Owner.Creature, 0m, choiceContext, cardPlay.Target);
        await CreatureCmd.GainBlock(base.Owner.Creature, damageResults.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage), ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
