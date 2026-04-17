//****************** 代码文件申明 ***********************
//* 文件：AtlasManagerPatch
//* 作者：wheat
//* 创建时间：2026/04/17 星期五
//* 描述：Harmony Patch，追加加载模组 .sprites（不覆盖原版）
//*******************************************************
/*
using System.Text.RegularExpressions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Logging;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 原版流程：<see cref="AtlasResourceLoader"/> 通过路径 <c>res://images/atlases/{atlas}.sprites/{sprite}.tres</c>
/// 触发 <see cref="AtlasManager"/> 从 <c>res://images/atlases/{atlas}.tpsheet</c> 加载，并动态创建 <see cref="AtlasTexture"/>。
/// </summary>
/// <remarks>
/// 本补丁目标：不替换/覆盖原版图集/tpsheet 加载逻辑。
/// 当原版对某个 sprite 的加载失败时，额外尝试从模组生成的真实资源目录加载：
/// <c>res://images/atlas/bilibiliacgn_{atlas}.sprites/{sprite}.tres</c>。
/// </remarks>
[HarmonyPatch]
public static class AtlasManagerPatch
{
    private static int _patchIntroLogged;
    private static int _fallbackLoadInfoHits;

    /// <summary>模组图集根目录（注意是 atlas 不是 atlases）。</summary>
    public const string ModAtlasBasePath = "res://images/atlas/";

    /// <summary>文件名 / 文件夹名上的前缀，与 Photoshop 导出脚本一致。</summary>
    public const string ModAtlasNamePrefix = "bilibiliacgn_";

    /// <summary>仅对这些逻辑图集名开启“追加加载 .sprites/.tres”逻辑。</summary>
    private static readonly HashSet<string> ModOverlayAtlasNames =
    [
        "power_atlas",
        "card_atlas",
        "relic_atlas",
        "relic_outline_atlas",
    ];

    /// <summary>
    /// 原版 sprite 虚拟路径：<c>res://images/atlases/{atlas}.sprites/{sprite}.tres</c>。
    /// 捕获组 1 = atlas 逻辑名（如 power_atlas）；组 2 = sprite stem（可含子目录，不含 .tres）。
    /// </summary>
    private static readonly Regex VanillaSpriteTresPath = new(
        @"^res://images/atlases/([^/]+)\.sprites/(.+)\.tres$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    private static bool IsModOverlayAtlas(string atlasName) => ModOverlayAtlasNames.Contains(atlasName);

    private static void TryLogPatchIntroOnce()
    {
        if (Interlocked.Exchange(ref _patchIntroLogged, 1) != 0)
            return;

        Log.Info(
            "[BiliBiliACGN] AtlasManagerPatch: 已注入（追加模式，不覆盖原版）。" +
            "当原版 res://images/atlases/{atlas}.sprites/{sprite}.tres 加载失败时，" +
            "将额外尝试加载 res://images/atlas/" + ModAtlasNamePrefix + "{atlas}.sprites/{sprite}.tres（真实文件）。"
        );
    }

    private static string GetModSpriteTresPath(string atlasName, string spriteStem) =>
        ModAtlasBasePath + ModAtlasNamePrefix + atlasName + ".sprites/" + spriteStem + ".tres";

    /// <summary>
    /// 追加补丁：当原版 AtlasResourceLoader._Exists 返回 false 时，检查对应模组真实 .tres 是否存在。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AtlasResourceLoader), "_Exists")]
    public static void AtlasResourceLoader_Exists_Postfix(string path, ref bool __result)
    {
        if (__result)
            return;

        var m = VanillaSpriteTresPath.Match(path);
        if (!m.Success)
            return;

        var atlasName = m.Groups[1].Value;
        var spriteStem = m.Groups[2].Value;
        if (!IsModOverlayAtlas(atlasName))
            return;

        TryLogPatchIntroOnce();

        var modPath = GetModSpriteTresPath(atlasName, spriteStem);
        if (ResourceLoader.Exists(modPath))
        {
            __result = true;
            Log.Debug($"[BiliBiliACGN] AtlasResourceLoaderPatch: _Exists 命中模组 sprite：{atlasName}/{spriteStem} -> {modPath}");
        }
    }

    /// <summary>
    /// 追加补丁：当原版 AtlasResourceLoader._Load 返回缺失（通常为 7L）或空时，尝试直接加载模组真实 .tres。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AtlasResourceLoader), "_Load")]
    public static void AtlasResourceLoader_Load_Postfix(
        string path,
        string originalPath,
        bool useSubThreads,
        int cacheMode,
        ref Variant __result
    )
    {
        // 原版成功返回 AtlasTexture / Texture2D 时，不干预。
        if (__result.VariantType != Variant.Type.Int && __result.VariantType != Variant.Type.Nil)
            return;

        var m = VanillaSpriteTresPath.Match(path);
        if (!m.Success)
            return;

        var atlasName = m.Groups[1].Value;
        var spriteStem = m.Groups[2].Value;
        if (!IsModOverlayAtlas(atlasName))
            return;

        TryLogPatchIntroOnce();

        var modPath = GetModSpriteTresPath(atlasName, spriteStem);
        if (!ResourceLoader.Exists(modPath))
            return;

        // 直接加载真实 .tres（AtlasTexture），不依赖 AtlasManager/tpsheet。
        var res = ResourceLoader.Load(modPath);
        if (res == null)
            return;

        var n = Interlocked.Increment(ref _fallbackLoadInfoHits);
        if (n <= 3)
            Log.Info($"[BiliBiliACGN] AtlasResourceLoaderPatch: 原版缺失，已回退加载模组 sprite #{n}: {atlasName}/{spriteStem} -> {modPath}");
        else
            Log.Debug($"[BiliBiliACGN] AtlasResourceLoaderPatch: 回退加载模组 sprite: {atlasName}/{spriteStem} -> {modPath}");

        __result = res;
    }
}
*/