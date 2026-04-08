//****************** 代码文件申明 ***********************
//* 文件：QianDaJiang(千大将)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：激发你最右侧的充能球X/X+1次。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class QianDaJiang : CardBaseModel
{
    private const int energyCost = -1; // X 费
    protected override bool HasEnergyCostX => true;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];

    public QianDaJiang() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 激发最右侧充能球 X / X+1 次
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		int evokeCount = ResolveEnergyXValue();
		if (base.IsUpgraded)
		{
			evokeCount++;
		}
		for (int i = 0; i < evokeCount; i++)
		{
			await OrbCmd.EvokeNext(choiceContext, base.Owner, i == evokeCount - 1);
			await Cmd.Wait(0.25f);
		}
    }

    protected override void OnUpgrade()
    {
    }
}
