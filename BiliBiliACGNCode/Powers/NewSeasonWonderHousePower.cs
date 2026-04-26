//****************** 代码文件申明 ***********************
//* 文件：NewSeasonWonderHousePower(新番妙妙屋)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：在你的回合开始时，[gold]生成[/gold] Amount 个随机充能球。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonWonderHousePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {   
        // 只在自己回合开始时触发
        if(player.Creature != base.Owner) return;
        int amt = base.Amount;
        var combatState = base.Owner.CombatState;
        if(combatState == null) return;
        // 生成充能球
        for(int i = 0; i < amt; i++)
        {
            await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(combatState), player);
        }
    }


}
