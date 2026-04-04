//****************** 代码文件申明 ***********************
//* 文件：CombatHelper
//* 作者：wheat
//* 创建时间：2026/04/04 10:00:00 星期六
//* 描述：战斗辅助类
//*******************************************************

using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class CombatHelper
{
    /// <summary>
    /// 无中生有的情况下使用的PlayerChoiceContext
    /// </summary>
    /// <returns></returns>
    public static PlayerChoiceContext GetTemporaryPlayerChoiceContext() => new BlockingPlayerChoiceContext();
    /*
    // 弃用的
        PlayerChoiceContext? choiceContext = null;
        GameAction? running = RunManager.Instance.ActionExecutor.CurrentlyRunningAction;
        if (running is PlayCardAction playCard && playCard.PlayerChoiceContext != null)
        {
            choiceContext = playCard.PlayerChoiceContext;
        }
        else if (running is GenericHookGameAction hookAction && hookAction.ChoiceContext != null)
        {
            // 若叠层来自「带 HookPlayerChoiceContext 的 hook 动作」（不是普通出牌）
            choiceContext = hookAction.ChoiceContext;
        }
    */
}