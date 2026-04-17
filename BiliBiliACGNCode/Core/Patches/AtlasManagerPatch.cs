//****************** 代码文件申明 ***********************
//* 文件：AtlasManagerLogPatch
//* 作者：wheat
//* 创建时间：2026/04/17 星期五
//* 描述：Harmony Prefix，记录 AtlasManager.GetSprite 调用参数
//*******************************************************
/*
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Assets;
namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 记录运行时对 MegaCrit.Sts2.Core.Assets.AtlasManager.GetSprite 的调用（atlasName / spriteName）。
/// </summary>
[HarmonyPatch(typeof(AtlasManager))]
public static class AtlasManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GetSprite")]
    public static void GetSprite_Prefix(string atlasName, string spriteName)
    {
        Log.Info($"[AtlasManager.GetSprite] atlasName={atlasName}, spriteName={spriteName}");
    }
}

*/