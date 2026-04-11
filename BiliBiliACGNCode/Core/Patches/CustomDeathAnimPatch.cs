/*
//****************** 代码文件申明 ***********************
//* 文件：CustomDeathAnimPatch
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：Harmony Postfix，自定义死亡动画
//*******************************************************

using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Helpers;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

 [HarmonyPatch(typeof(NCreature))]
public static class CustomDeathAnimPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("StartDeathAnim")]
    public static void StartDeathAnim_Postfix(NCreature __instance)
    {
        if (__instance.Entity is not { IsPlayer: true }) return;

        var visuals = __instance.Visuals;
        if (visuals == null) return;

        // 异步播放死亡动画
        TaskHelper.RunSafely(PlayDeathAnimationAsync(visuals));
    }

    private static async Task PlayDeathAnimationAsync(Node visuals)
    {
        // 等待一帧，确保节点稳定
        await visuals.ToSignal(visuals.GetTree(), SceneTree.SignalName.ProcessFrame);
        if (!GodotObject.IsInstanceValid(visuals)) return;

        // 查找 AnimationPlayer（递归）
        var animationPlayer = FindAnimationPlayer(visuals);
        if (animationPlayer == null) return;

        // 播放 die 动画（如果有）
        if (animationPlayer.HasAnimation("die"))
        {
            animationPlayer.Play("die");
            // 等待动画结束（可选，如果动画结束后需要移除节点）
            await animationPlayer.ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
            // 动画结束后，可以调用 visuals.QueueFree() 或保持原样
            // 注意：原版游戏可能在死亡后还会进行其他清理，这里不需要手动移除节点
        }
    }

    private static AnimationPlayer FindAnimationPlayer(Node node)
    {
        if (node is AnimationPlayer ap) return ap;
        foreach (var child in node.GetChildren())
        {
            var found = FindAnimationPlayer(child);
            if (found != null) return found;
        }
        return null;
    }
}
*/