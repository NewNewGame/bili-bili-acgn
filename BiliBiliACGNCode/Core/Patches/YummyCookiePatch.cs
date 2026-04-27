//****************** 代码文件申明 ***********************
//* 文件：YummyCookiePatch
//* 作者：wheat
//* 创建时间：2026/04/27 星期一
//* 描述：美味饼干补丁为自定义角色提供图标
//*******************************************************

using System.Reflection;
using BiliBiliACGN.BiliBiliACGNCode.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(YummyCookie))]
public static class YummyCookiePatch
{
    // _cachedRandomCharacter反射字段
    private static readonly Lazy<FieldInfo?> CachedRandomCharacterField = new(() =>
        AccessTools.DeclaredField(typeof(YummyCookie), "_cachedRandomCharacter"));
    /// <summary>
    /// 获取美味饼干图标
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="__result"></param>
    [HarmonyPostfix]
    [HarmonyPatch("get_IconBaseName")]
    public static void GetIconBaseName_Postfix(YummyCookie __instance, ref string __result)
    {
        // 获取角色
        CharacterModel characterModel;
        if (__instance.IsCanonical || __instance.Owner == null)
        {
            if (CachedRandomCharacterField.Value?.GetValue(__instance) == null)
            {
                CachedRandomCharacterField.Value?.SetValue(__instance, Rng.Chaotic.NextItem(ModelDb.AllCharacters));
            }

            characterModel = CachedRandomCharacterField.Value?.GetValue(__instance) as CharacterModel;
        }
        else
        {
            characterModel = __instance.Owner.Character;
        }
        if(characterModel == null) return;
        // 美味饼干图标配置
        switch(characterModel){
            case FunShikiCharacter:
                __result = "yummy_cookie_funshiki";
                break;
            case BottleCharacter:
                __result = "yummy_cookie_bottle";
                break;
        }
    }
}