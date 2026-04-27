//****************** 代码文件申明 ***********************
//* 文件：PenguinPajamas(企鹅睡衣)
//* 作者：wheat
//* 创建时间：2026/04/27 09:37:00 星期一
//* 描述：每2个回合，获得1个充能球栏位。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(FunShikiRelicPool))]
public sealed class PenguinPajamas : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    private int _turns;
    public int Turns{
        get{
            return _turns;
        }
        set{
            _turns = value;
            InvokeDisplayAmountChanged();
        }
    }
    public override int DisplayAmount => Turns;
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner)
        {
            return;
        }

        Turns = (Turns + 1) % 2;
        if (Turns == 0)
        {
            Flash();
            await OrbCmd.AddSlots(base.Owner, 1);
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Turns = 0;
        return Task.CompletedTask;
    }
}

