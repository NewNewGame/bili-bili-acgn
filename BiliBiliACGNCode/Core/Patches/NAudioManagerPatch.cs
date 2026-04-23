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
using System.Collections.Generic;

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
    /// 在 NAudioManager PlayOneShot 时，转发到自定义 AudioUtils（对象池版本）。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(
        "PlayOneShot",
        new[] { typeof(string), typeof(Dictionary<string, float>), typeof(float) }
    )]
    private static void PlayOneShot_Postfix(string path, Dictionary<string, float> parameters, float volume = 1f)
    {
        AudioUtils.PlayOneShotSfx(path, volume);
    }
}

