//****************** 代码文件申明 ***********************
//* 文件：SilverSpoon(银勺)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：你的[gold]消耗牌[/gold]不在被[gold]消耗[/gold]，而是进入[gold]弃牌堆[/gold]。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SilverSpoon : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    /// <summary>
    /// 你的[gold]消耗牌[/gold]不在被[gold]消耗[/gold]，而是进入[gold]弃牌堆[/gold]。
    /// </summary>
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
	{
		if (card.Owner != base.Owner)
		{
			return (pileType, position);
		}
        if(!card.Keywords.Contains(CardKeyword.Exhaust)){
            return (pileType, position);
        }
		return (PileType.Discard, CardPilePosition.Bottom);
	}

}
