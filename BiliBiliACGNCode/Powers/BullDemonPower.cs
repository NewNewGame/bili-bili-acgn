//****************** 代码文件申明 ***********************
//* 文件：BullDemonPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 牛魔形态
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BullDemonPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每打出一张有一说一获得 Amount 点「唐氏」
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 不是自己打的牌，不处理
        if(cardPlay.Card.Owner != base.Owner.Player) return;
        // 如果有有一说一，那就添加唐氏
        if(cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY)){
            await PowerCmd.Apply<TangShiPower>(base.Owner, base.Amount, base.Owner, null);
        }else{
            // 如果没有有一说一，那就添加有一说一
            cardPlay.Card.AddKeyword(CustomKeyWords.YYSY);
        }
        
    }
}
