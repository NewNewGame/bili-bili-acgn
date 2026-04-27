//****************** 代码文件申明 ***********************
//* 文件：FffGroupClothes(fff团衣服)
//* 作者：wheat
//* 创建时间：2026/04/27 17:30:00 星期一
//* 描述：拾起时，选择一张攻击牌，为它附魔启动。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FffGroupClothes : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override bool HasUponPickupEffect => true;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<StartUp>();


    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
		StartUp canonicalMomentum = ModelDb.Enchantment<StartUp>();
		foreach (CardModel item in await CardSelectCmd.FromDeckGeneric(base.Owner, prefs, (CardModel c) => c.Type == CardType.Attack && c.Enchantment == null))
		{
			CardCmd.Enchant(canonicalMomentum.ToMutable(), item, 1m);
			CardCmd.Preview(item);
		}
    }
}

