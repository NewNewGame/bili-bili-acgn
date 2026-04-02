//****************** 代码文件申明 ***********************
//* 文件：EmptyStomach
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：空腹（诅咒）。牌面：回合结束时若在手牌则失去生命—。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(CurseCardPool))]
public sealed class EmptyStomach : CardBaseModel
{
    private const int energyCost = -1;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Curse;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;
    public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HpLoss", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    public EmptyStomach() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

	public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
	{
		await Cmd.Wait(0.25f);
		await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
	}
    protected override void OnUpgrade()
    {
    }
}
