//****************** 代码文件申明 ***********************
//* 文件：DetectiveWatch(侦探手表)
//* 作者：wheat
//* 创建时间：2026/04/27 17:30:40 星期一
//* 描述：战斗开始时，击晕所有敌人一个回合。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class DetectiveWatch : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Stun)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            var combatState = player.Creature.CombatState;
            if(combatState?.RoundNumber == 1)
            {
                Flash();
                foreach(var enemy in combatState.HittableEnemies)
                {
                    await CreatureCmd.Stun(enemy);
                }
            }
        }
    }

}

