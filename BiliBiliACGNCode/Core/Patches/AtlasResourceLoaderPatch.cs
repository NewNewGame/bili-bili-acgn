//****************** 代码文件申明 ***********************
//* 文件：AtlasResourceLoaderPatch
//* 作者：wheat
//* 创建时间：2026/04/27
//* 描述：Harmony Postfix，Atlas资源加载器
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(AtlasResourceLoader))]
public static class AtlasResourceLoaderPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("_Load")]
    public static void _Load_Postfix(string path, string originalPath, bool useSubThreads, int cacheMode, ref Variant __result)
    {
        // 为自定义角色提供图标
        if(path.EndsWith("yummy_cookie_funshiki.tres") || path.EndsWith("yummy_cookie_bottle.tres"))
        {
            __result = GetYummyCookieTexture(path);
        }
    }
    private static Variant GetYummyCookieTexture(string originalPath)
    {
        string path = $"res://BiliBiliACGN/images/relics/big/{originalPath}.tres";
        if (ResourceLoader.Exists(path))
		{
			Texture2D texture2D = ResourceLoader.Load<Texture2D>(path, null, ResourceLoader.CacheMode.Reuse);
			if (texture2D != null)
			{
				return texture2D;
			}
		}
		return 7L;
    }
}