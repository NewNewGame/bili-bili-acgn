//****************** 代码文件申明 ***********************
//* 文件：NAudioManagerPatch
//* 作者：wheat
//* 创建时间：2026/04/23 00:00:00 星期四
//* 描述：在 NAudioManager EnterTree 时设置 AudioUtils 默认父节点
//*******************************************************

using System.Reflection;
using Godot;
using HarmonyLib;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(NAudioManager))]
public static class NAudioManagerPatch
{
    /// <summary>
    /// 在 NAudioManager EnterTree 时设置 AudioUtils 默认父节点
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch("_EnterTree")]
    private static void EnterTree_Postfix(object __instance)
    {
        if (__instance is not Node node)
            return;

        AudioUtils.SetDefaultAudioManagerParent(node);
    }
    /// <summary>
    /// 在 NAudioManager PlayOneShot 时使用 AudioUtils 播放音效
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <param name="volume"></param>
    [HarmonyPostfix]
    [HarmonyPatch("PlayOneShot")]
    private static void PlayOneShot_Postfix(object __instance, string path, System.Collections.Generic.Dictionary<string, float> parameters, float volume = 1f)
    {
        if (__instance is not Node node)
            return;

        AudioUtils.PlayOneShotSfx(path, volume);
    }
}

