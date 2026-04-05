//****************** 代码文件申明 ***********************
//* 文件：DeathNoteE(死亡笔记E)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：死亡笔记链 E：虚弱、抽牌堆加入 A。消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class DeathNoteE : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<ArtifactPower>(),HoverTipFactory.FromCard<DeathNoteA>()];

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Weak", 99m),
    ];

    public DeathNoteE() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 移除 AI；施加虚弱；抽牌堆加入 DeathNoteA
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if(cardPlay.Target.HasPower<ArtifactPower>())
        {
            await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        }
        // 施加虚弱
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars["Weak"].BaseValue, base.Owner.Creature, this);
        // 弃牌堆加入 DeathNoteA
        CardModel card = base.CombatState.CreateCard<DeathNoteA>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true));
		await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
