//****************** 代码文件申明 ***********************
//* 文件：HumanRaceChessPiece(人族种族棋子)
//* 作者：wheat
//* 创建时间：2026/04/27 17:31:00 星期一
//* 描述：可以将卡牌奖励转变为 +2 最大生命值。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class HumanRaceChessPiece : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 2m)];


    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        // 如果玩家不是当前玩家，则返回false
        if (base.Owner != player)
		{
			return false;
		}
        // 新增一个备选项，将本次卡牌奖励替换为「+2 最大生命值」。
		alternatives.Add(new CardRewardAlternative("HUMAN_RACE_CHESS_PIECE", OnSacrificeSynchronized, PostAlternateCardRewardAction.DismissScreenAndRemoveReward));
        return true;
    }
    private async Task OnSacrificeSynchronized()
	{
		Flash();
        int value = (int)base.DynamicVars["Amount"].BaseValue;
        await CreatureCmd.SetMaxHp(base.Owner.Creature, base.Owner.Creature.MaxHp + value);
        await CreatureCmd.Heal(base.Owner.Creature, value);
	}
}

