//****************** 代码文件申明 ***********************
//* 文件：AtlasManagerPatch
//* 作者：wheat
//* 创建时间：2026/04/17 星期五
//* 描述：Harmony Patch，使 bilibiliacgn 前缀图集从 res://images/atlas/ 加载并扩展虚拟 .sprites 路径
//*******************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Logging;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 原版 <see cref="AtlasManager"/> 只认 <c>res://images/atlases/{atlasName}.tpsheet</c>；
/// 模组图集放在 <c>res://images/atlas/bilibiliacgn_{atlasName}.tpsheet</c>（与 Godot 插件导出约定一致）。
/// 同时扩展 <see cref="AtlasResourceLoader"/> 对 <c>res://images/atlas/.../*.sprites/*.tres</c> 的识别与解析。
/// </summary>
/// <remarks>
/// 游戏程序集里的 <c>_atlases</c>、嵌套类型 <c>AtlasData</c>/<c>SpriteInfo</c>、<c>_jsonOptions</c> 等可能是 <c>private</c>，
/// 因此本类通过 <see cref="AccessTools"/> 与 <see cref="Type.GetNestedType"/> 等反射访问，避免编译期绑定到不可见符号。
/// </remarks>
[HarmonyPatch]
public static class AtlasManagerPatch
{
    /// <summary>是否已输出「补丁已启用」类说明（避免重复刷屏）。</summary>
    private static int _patchIntroLogged;

    /// <summary>已对哪些逻辑图集输出过「未找到模组 tpsheet」提示。</summary>
    private static readonly HashSet<string> MissingModTpsheetLogged = [];

    private static int _modIsSpritePathInfoHits;
    private static int _modParsePathInfoHits;

    /// <summary>模组图集根目录（注意是 atlas 不是 atlases）。</summary>
    public const string ModAtlasBasePath = "res://images/atlas/";

    /// <summary>文件名 / 文件夹名上的前缀，与 Photoshop 导出脚本一致。</summary>
    public const string ModAtlasNamePrefix = "bilibiliacgn_";

    /// <summary>仅对这些逻辑图集名尝试加载模组覆盖（与游戏内 atlas 键一致）。</summary>
    private static readonly HashSet<string> ModOverlayAtlasNames =
    [
        "power_atlas",
        "card_atlas",
        "relic_atlas",
        "relic_outline_atlas",
    ];

    /// <summary>
    /// 虚拟 sprite 路径：<c>res://images/atlas/{图集文件夹}.sprites/{子图名}.tres</c>。
    /// 捕获组 1 = 图集文件夹名（如 bilibiliacgn_power_atlas）；组 2 = 子图 stem（不含 .tres）。
    /// </summary>
    private static readonly Regex ModSpriteTresPath = new(
        @"^res://images/atlas/([^/]+)\.sprites/(.+)\.tres$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    /// <summary>
    /// 在原版 <see cref="AtlasManager.LoadAtlasInternal"/> 之前执行：若存在模组 tpsheet 则加载并跳过原版。
    /// </summary>
    /// <returns>
    /// Harmony Prefix：<c>true</c> 继续执行原版；<c>false</c> 跳过原版（表示已由本方法完成加载或放弃覆盖）。
    /// </returns>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AtlasManager), "LoadAtlasInternal")]
    public static bool LoadAtlasInternal_Prefix(string atlasName)
    {
        // 非白名单图集不拦截，走原版路径（res://images/atlases/）。
        if (!ModOverlayAtlasNames.Contains(atlasName))
            return true;

        TryLogPatchIntroOnce();

        // 与 Godot 插件导出一致：逻辑名 power_atlas -> 文件 bilibiliacgn_power_atlas.tpsheet
        var tpsheetPath = ModAtlasBasePath + ModAtlasNamePrefix + atlasName + ".tpsheet";
        if (!Godot.FileAccess.FileExists(tpsheetPath))
        {
            lock (MissingModTpsheetLogged)
            {
                if (MissingModTpsheetLogged.Add(atlasName))
                {
                    Log.Info(
                        $"[BiliBiliACGN] AtlasManagerPatch: 未找到模组 tpsheet，本图集沿用原版目录（images/atlases）。atlas={atlasName}，期望文件={tpsheetPath}"
                    );
                }
            }

            return true;
        }

        Log.Info($"[BiliBiliACGN] AtlasManagerPatch: 检测到模组 tpsheet，开始加载。atlas={atlasName}，path={tpsheetPath}");

        if (TryLoadModAtlasFromTpsheet(atlasName, tpsheetPath))
        {
            // 已成功写入 _atlases[atlasName]，勿再执行原版（避免覆盖或重复读 atlases 目录）。
            return false;
        }

        Log.Warn($"[BiliBiliACGN] AtlasManager: 模组 tpsheet 存在但解析失败，回退原版：{tpsheetPath}");
        return true;
    }

    /// <summary>
    /// 按原版 LoadAtlasInternal 等价流程：读 tpsheet JSON、按页加载 PNG、建 SpriteMap，字典键仍为游戏逻辑名（如 power_atlas）。
    /// </summary>
    private static bool TryLoadModAtlasFromTpsheet(string logicalAtlasName, string tpsheetPath)
    {
        using var fileAccess = Godot.FileAccess.Open(tpsheetPath, Godot.FileAccess.ModeFlags.Read);
        if (fileAccess == null)
        {
            Log.Warn($"[BiliBiliACGN] AtlasManagerPatch: 无法打开 tpsheet 文件：{tpsheetPath}");
            return false;
        }

        var jsonText = fileAccess.GetAsText();
        var jsonOptions = GetAtlasManagerJsonOptions();
        var tpSheetData = JsonSerializer.Deserialize<TpSheetData>(jsonText, jsonOptions);
        if (tpSheetData?.Textures == null)
        {
            Log.Warn($"[BiliBiliACGN] AtlasManagerPatch: tpsheet JSON 解析失败或 textures 为空：{tpsheetPath}");
            return false;
        }

        var pageTextures = new Dictionary<string, Texture2D>();

        var spriteInfoType = GetNestedType(typeof(AtlasManager), "SpriteInfo")
            ?? throw new InvalidOperationException("AtlasManager.SpriteInfo 类型未找到");
        var spriteMap = CreateStringKeyedDictionary(spriteInfoType);

        foreach (var texture in tpSheetData.Textures)
        {
            var imageName = texture.Image;
            if (string.IsNullOrEmpty(imageName))
                continue;

            // tpsheet 里 image 多为相对文件名，与原版一样拼在图集根目录后。
            var imagePath = ModAtlasBasePath + imageName;
            var texture2D = ResourceLoader.Load<Texture2D>(imagePath, null, ResourceLoader.CacheMode.Reuse);
            if (texture2D == null)
            {
                Log.Warn($"[BiliBiliACGN] AtlasManager: 无法加载图集页纹理：{imagePath}");
                continue;
            }

            pageTextures[imageName] = texture2D;
            if (texture.Sprites == null)
                continue;

            foreach (var sprite in texture.Sprites)
            {
                // 与 AtlasManager 一致：去掉 .png 后缀等，作为 GetSprite 第二参查找键。
                var key = InvokeNormalizeSpriteKey(sprite.Filename);
                var spriteInfo = CreateSpriteInfo(spriteInfoType, texture2D, sprite);
                ((IDictionary)spriteMap)[key] = spriteInfo!;
            }
        }

        if (((IDictionary)spriteMap).Count == 0)
            return false;

        var atlasDataType = GetNestedType(typeof(AtlasManager), "AtlasData")
            ?? throw new InvalidOperationException("AtlasManager.AtlasData 类型未找到");
        var atlasData = CreateAtlasData(atlasDataType, tpSheetData, pageTextures, spriteMap);

        // 键必须是游戏内使用的 atlasName（如 card_atlas），以便 GetSprite("card_atlas", ...) 命中。
        SetAtlasDataInManager(logicalAtlasName, atlasData);

        var spriteCount = ((IDictionary)spriteMap).Count;
        Log.Info(
            $"[BiliBiliACGN] AtlasManagerPatch: 模组图集加载完成。logical={logicalAtlasName}，tpsheet={tpsheetPath}，页纹理数={pageTextures.Count}，sprite 条目数={spriteCount}"
        );
        return true;
    }

    private static void TryLogPatchIntroOnce()
    {
        if (Interlocked.Exchange(ref _patchIntroLogged, 1) != 0)
            return;

        Log.Info(
            "[BiliBiliACGN] AtlasManagerPatch: 已注入。白名单图集若存在 "
                + ModAtlasBasePath
                + ModAtlasNamePrefix
                + "{power|card|relic...}_atlas.tpsheet 则从模组目录加载；"
                + "AtlasResourceLoader 同时识别 "
                + ModAtlasBasePath
                + "*.sprites/*.tres 虚拟路径。"
        );
    }

    private static JsonSerializerOptions GetAtlasManagerJsonOptions()
    {
        var field = AccessTools.Field(typeof(AtlasManager), "_jsonOptions");
        if (field?.GetValue(null) is JsonSerializerOptions opts)
            return opts;

        // 若字段名/可见性变化，仍保证能解析 tpsheet（与原版常用设置一致）。
        Log.Debug("[BiliBiliACGN] AtlasManagerPatch: 未反射到 AtlasManager._jsonOptions，使用备用 JsonSerializerOptions。");
        return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private static string InvokeNormalizeSpriteKey(string filename)
    {
        var method = AccessTools.Method(typeof(AtlasManager), "NormalizeSpriteKey", new[] { typeof(string) });
        if (method == null)
            return filename;

        return (string)method.Invoke(null, new object[] { filename })!;
    }

    private static object CreateStringKeyedDictionary(Type valueType)
    {
        var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), valueType);
        return Activator.CreateInstance(dictType)
            ?? throw new InvalidOperationException("无法创建 SpriteMap 字典实例");
    }

    private static object CreateSpriteInfo(Type spriteInfoType, Texture2D atlasPage, object tpSheetSprite)
    {
        var spriteArgType = tpSheetSprite.GetType();
        var ctor = AccessTools.Constructor(spriteInfoType, new[] { typeof(Texture2D), spriteArgType });
        if (ctor == null)
        {
            foreach (var c in spriteInfoType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var p = c.GetParameters();
                if (p.Length != 2)
                    continue;
                if (p[0].ParameterType != typeof(Texture2D))
                    continue;
                if (!p[1].ParameterType.IsAssignableFrom(spriteArgType))
                    continue;
                ctor = c;
                break;
            }
        }

        if (ctor == null)
            throw new InvalidOperationException("SpriteInfo 构造函数未找到");

        return ctor.Invoke(new[] { atlasPage, tpSheetSprite })!;
    }

    private static object CreateAtlasData(
        Type atlasDataType,
        TpSheetData tpSheet,
        Dictionary<string, Texture2D> pageTextures,
        object spriteMap
    )
    {
        // 避免 FormatterServices（在较新 .NET 上标记过时）；仅分配内存后由反射写入字段/属性。
        var instance = RuntimeHelpers.GetUninitializedObject(atlasDataType);

        // 嵌套类型属性名与反编译一致；若游戏改名，可在此处补充别名尝试。
        SetPropertyOrField(atlasDataType, instance, "TpSheet", tpSheet);
        SetPropertyOrField(atlasDataType, instance, "PageTextures", pageTextures);
        SetPropertyOrField(atlasDataType, instance, "SpriteMap", spriteMap);

        return instance;
    }

    private static void SetPropertyOrField(Type declaringType, object target, string memberName, object? value)
    {
        var prop = AccessTools.Property(declaringType, memberName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(target, value);
            return;
        }

        var field = AccessTools.Field(declaringType, memberName);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        throw new InvalidOperationException($"AtlasData 上未找到可写成员: {memberName}");
    }

    private static void SetAtlasDataInManager(string logicalAtlasName, object atlasData)
    {
        var field = AccessTools.Field(typeof(AtlasManager), "_atlases");
        if (field?.GetValue(null) is not IDictionary dict)
            throw new InvalidOperationException("AtlasManager._atlases 字段未找到或类型不是字典");

        dict[logicalAtlasName] = atlasData;
    }

    private static Type? GetNestedType(Type declaringType, string nestedTypeName)
    {
        return declaringType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
    }

    /// <summary>
    /// 原版 <see cref="AtlasResourceLoader.IsSpritePath"/> 只认 <c>res://images/atlases/</c>；此处把模组虚拟路径也标成 sprite 路径。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AtlasResourceLoader), "IsSpritePath")]
    public static void AtlasResourceLoader_IsSpritePath_Postfix(string path, ref bool __result)
    {
        if (__result)
            return;

        if (path.StartsWith(ModAtlasBasePath, StringComparison.Ordinal)
            && path.Contains(".sprites/", StringComparison.Ordinal)
            && path.EndsWith(".tres", StringComparison.Ordinal))
        {
            __result = true;
            var n = Interlocked.Increment(ref _modIsSpritePathInfoHits);
            if (n <= 3)
                Log.Info($"[BiliBiliACGN] AtlasResourceLoaderPatch: 识别为模组虚拟 sprite 路径（IsSpritePath） #{n}: {path}");
            else if (n == 4)
                Log.Info("[BiliBiliACGN] AtlasResourceLoaderPatch: 后续同类路径仅打 Debug，避免日志刷屏。");
            else
                Log.Debug($"[BiliBiliACGN] AtlasResourceLoaderPatch: IsSpritePath 模组路径 #{n}: {path}");
        }
    }

    /// <summary>
    /// 原版 <see cref="AtlasResourceLoader.ParsePath"/> 正则只匹配 atlases；此处在解析失败时补全 atlas 目录下的路径。
    /// 将文件夹名 <c>bilibiliacgn_power_atlas</c> 还原为逻辑名 <c>power_atlas</c>，供 <see cref="AtlasManager.LoadAtlas"/> / GetSprite 使用。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AtlasResourceLoader), "ParsePath")]
    public static void AtlasResourceLoader_ParsePath_Postfix(string path, ref (string? AtlasName, string? SpriteName) __result)
    {
        // 已由原版正则解析成功则不改。
        if (__result.AtlasName != null && __result.SpriteName != null)
            return;

        var m = ModSpriteTresPath.Match(path);
        if (!m.Success)
            return;

        // 例：bilibiliacgn_power_atlas.sprites/foo -> folderStem=bilibiliacgn_power_atlas, spriteStem=foo
        var folderStem = m.Groups[1].Value;
        var spriteStem = m.Groups[2].Value;

        var logicalAtlas =
            folderStem.StartsWith(ModAtlasNamePrefix, StringComparison.Ordinal)
                ? folderStem.Substring(ModAtlasNamePrefix.Length)
                : folderStem;

        __result = (logicalAtlas, spriteStem);

        var n = Interlocked.Increment(ref _modParsePathInfoHits);
        if (n <= 3)
        {
            Log.Info(
                $"[BiliBiliACGN] AtlasResourceLoaderPatch: ParsePath 模组补全 #{n}: path={path} -> atlas={logicalAtlas}, sprite={spriteStem}"
            );
        }
        else if (n == 4)
        {
            Log.Info("[BiliBiliACGN] AtlasResourceLoaderPatch: 后续 ParsePath 模组补全仅打 Debug。");
        }
        else
        {
            Log.Debug($"[BiliBiliACGN] AtlasResourceLoaderPatch: ParsePath 模组补全 #{n}: atlas={logicalAtlas}, sprite={spriteStem}");
        }
    }
}
