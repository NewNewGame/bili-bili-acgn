//****************** 代码文件申明 ***********************
//* 文件：QianHuDiary
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：千户的日记 — 跳过卡牌奖励则回合结束获得「跳过次数×Amount」格挡（逻辑待实现）
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class QianHuDiary : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    /// <summary>格挡倍率中的常数 2（即 n×2 中的 2）。</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 2m)];

	private int _keepDiary;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			return BILIBILIACGN_QHD_KeepDiary * (int)base.DynamicVars["Amount"].BaseValue;
		}
	}

	[SavedProperty]
	public int BILIBILIACGN_QHD_KeepDiary
	{
		get
		{
			return _keepDiary;
		}
		set
		{
			AssertMutable();
			_keepDiary = value;
			InvokeDisplayAmountChanged();
		}
	}

	public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
	{
		if (base.Owner != player)
		{
			return false;
		}
		alternatives.Add(new CardRewardAlternative("KEEP_DIARY", OnSacrificeSynchronized, PostAlternateCardRewardAction.DismissScreenAndRemoveReward));
		return true;
	}

	private async Task OnSacrificeSynchronized()
	{
		BILIBILIACGN_QHD_KeepDiary++;
		Flash();
	}
    /// <summary>
    /// 回合结束时，给玩家护盾
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if(side == base.Owner.Creature.Side && BILIBILIACGN_QHD_KeepDiary > 0)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner.Creature, BILIBILIACGN_QHD_KeepDiary * (int)base.DynamicVars["Amount"].BaseValue, ValueProp.Unpowered, null);
        }
    }

}
