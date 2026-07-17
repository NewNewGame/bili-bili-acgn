//****************** 代码文件申明 ***********************
//* 文件：SilverSpoon(银勺)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：你的[gold]消耗牌[/gold]不在被[gold]消耗[/gold]，而是进入[gold]弃牌堆[/gold]。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SilverSpoon : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    /// <summary>
    /// 你的[gold]消耗牌[/gold]不在被[gold]消耗[/gold]，而是进入[gold]弃牌堆[/gold]。
    /// </summary>
    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources, CardLocation cardLocation)
    {
        if (card.Owner != base.Owner)
		{
			return cardLocation;
		}
        // 不是消耗牌或着是诅咒牌的话不进入弃牌堆
        if(!card.Keywords.Contains(CardKeyword.Exhaust) || card.Rarity == CardRarity.Curse){
            return cardLocation;
        }
        // 进入弃牌堆
		return new CardLocation(cardLocation.player, PileType.Discard, CardPilePosition.Bottom);
    }


}
