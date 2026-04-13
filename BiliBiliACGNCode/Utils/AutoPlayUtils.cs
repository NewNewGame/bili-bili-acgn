//****************** 代码文件申明 ***********************
//* 文件：AutoPlayUtils
//* 作者：wheat
//* 创建时间：2026/04/14
//* 描述：自动出牌辅助类
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class AutoPlayUtils
{
    /// <summary>
    /// 安全自动出牌
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public static async Task AutoPlaySafely(PlayerChoiceContext choiceContext, CardModel card)
    {
        // 如果战斗状态为空或没有可攻击的敌人，则直接返回
        var combatState = card.CombatState;
        if(combatState == null || combatState.HittableEnemies.Count == 0){
            return;
        }
        await CardCmd.AutoPlay(choiceContext, card, null);
    }
}