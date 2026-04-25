//****************** 代码文件申明 ***********************
//* 文件：MillenniumPuzzle
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：千年积木
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MillenniumPuzzle : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Innate)];

    public override async Task AfterObtained()
    {
        CardModel cardModel = (await CardSelectCmd.FromDeckGeneric(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), player: base.Owner, filter: Filter)).FirstOrDefault();
        if(cardModel != null)
        {
            cardModel.AddKeyword(CardKeyword.Innate);
            CardCmd.Preview(cardModel);
        }
    }
    
    /// <summary>
    /// 过滤器
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
	private bool Filter(CardModel c)
	{
		return c.Type != CardType.Quest && !c.Keywords.Contains(CardKeyword.Innate);
	}
}
