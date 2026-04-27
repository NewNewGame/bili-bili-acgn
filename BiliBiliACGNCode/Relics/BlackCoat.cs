//****************** 代码文件申明 ***********************
//* 文件：BlackCoat(黑色大衣)
//* 作者：wheat
//* 创建时间：2026/04/27 17:31:40 星期一
//* 描述：拾起时，选择一张技能牌，为它添加启动。
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
public sealed class BlackCoat : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<StartUp>();

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
		StartUp canonicalMomentum = ModelDb.Enchantment<StartUp>();
		foreach (CardModel item in await CardSelectCmd.FromDeckGeneric(base.Owner, prefs, (CardModel c) => c.Type == CardType.Skill && c.Enchantment == null))
		{
			CardCmd.Enchant(canonicalMomentum.ToMutable(), item, 1m);
			CardCmd.Preview(item);
		}
    }
}

