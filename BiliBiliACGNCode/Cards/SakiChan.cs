//****************** 代码文件申明 ***********************
//* 文件：SakiChan(Saki酱)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：造成{Damage:diff()}点伤害，所有的Saki酱伤害加3/4。打出后将此牌复制加入抽牌堆。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class SakiChan : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    private decimal _extraDamageFromClawPlays;
	private decimal ExtraDamageFromClawPlays
	{
		get
		{
			return _extraDamageFromClawPlays;
		}
		set
		{
			AssertMutable();
			_extraDamageFromClawPlays = value;
		}
	}

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("IncreaseSakiChanDamage", 3m)
    ];

    public SakiChan() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害；将本牌复制置入抽牌堆
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars["Damage"].BaseValue)
        .FromCard(this)
        .Targeting(cardPlay.Target)
        .Execute(choiceContext);
        // 将本牌复制置入抽牌堆
        CardModel card = CreateClone();
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, true), 0.2f);
        await Cmd.Wait(0.2f);
        // 所有的Saki酱伤害加3/4
        IEnumerable<SakiChan> enumerable = base.Owner.PlayerCombatState.AllCards.OfType<SakiChan>();
		decimal baseValue = base.DynamicVars["IncreaseSakiChanDamage"].BaseValue;
		foreach (SakiChan item in enumerable)
		{
			item.BuffFromClawPlay(baseValue);
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
        base.DynamicVars["IncreaseSakiChanDamage"].UpgradeValueBy(1m);
    }
    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		base.DynamicVars.Damage.BaseValue += ExtraDamageFromClawPlays;
	}

	private void BuffFromClawPlay(decimal extraDamage)
	{
		base.DynamicVars.Damage.BaseValue += extraDamage;
		ExtraDamageFromClawPlays += extraDamage;
	}
}
