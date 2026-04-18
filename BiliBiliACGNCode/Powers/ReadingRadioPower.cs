//****************** 代码文件申明 ***********************
//* 文件：ReadingRadioPower(读书电台)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 读书电台 每当你抽到[blue]1[/blue]张带[gold]有一说一[/gold]的牌，获得[blue]1[/blue]点[gold]格挡[/gold]。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ReadingRadioPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == base.Owner && card.Keywords.Contains(CustomKeyWords.YYSY))
		{
			await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Unpowered, null);
		}
    }
}
