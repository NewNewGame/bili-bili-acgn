//****************** 代码文件申明 ***********************
//* 文件：NewSeasonTimeMachine(新番时光机)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：造成6/9点伤害，抽1张牌，女儿每进攻3次此牌返回手牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class NewSeasonTimeMachine : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(CustomeHoverTips.AttackOrb)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new CardsVar(1),
        new DynamicVar("AttacksPerReturn", 3m),
    ];

    public NewSeasonTimeMachine() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害，抽牌
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        // 抽牌
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // 如果施加者不是女儿，则返回
        if(dealer == null || dealer.Monster is not Itsuka) return;
        // 如果女儿每进攻3次此牌回手
        if(CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Count((DamageReceivedEntry e) => e.Dealer == dealer && e.HappenedThisTurn(base.CombatState)) % base.DynamicVars["AttacksPerReturn"].BaseValue == 0){
            await CardPileCmd.Add(this, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
