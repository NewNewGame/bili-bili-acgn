using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CardEditor.Shared;

/// <summary>
/// 将 Power 的 title/description/smartDescription 合并到游戏内 <c>powers.json</c>。
/// 键为 <c>{prefix}-{类名大写 snake_case}.title</c> / <c>.description</c> / <c>.smartDescription</c>。
/// 例如 prefix <c>BILIBILIACGN</c>、className <c>YYSYPower</c> → <c>BILIBILIACGN-Y_YS_Y_POWER.title</c>。
/// </summary>
public static class PowerLocalizationJsonMerger
{
    private static readonly JsonWriterOptions JsonWriteWriterOptions = new()
    {
        Indented = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    public static string BuildLocalizationKeyBase(string prefixUpper, string powerClassName)
    {
        var p = string.IsNullOrWhiteSpace(prefixUpper) ? "POWER" : prefixUpper.Trim().ToUpperInvariant();
        var seg = CardLocalizationJsonMerger.ClassNameToLocalizationSegment(powerClassName);
        return $"{p}-{seg}";
    }

    public static void MergePowerTexts(
        string path,
        string prefixUpper,
        string powerClassName,
        string title,
        string? description,
        string? smartDescription)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("路径不能为空。", nameof(path));
        if (string.IsNullOrWhiteSpace(powerClassName))
            throw new ArgumentException("className 不能为空。", nameof(powerClassName));

        var full = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var baseKey = BuildLocalizationKeyBase(prefixUpper, powerClassName.Trim());
        var titleKey = $"{baseKey}.title";
        var descKey = $"{baseKey}.description";
        var smartKey = $"{baseKey}.smartDescription";

        JsonObject root;
        if (File.Exists(full))
        {
            var text = File.ReadAllText(full);
            var parsed = JsonNode.Parse(text);
            root = parsed as JsonObject ?? new JsonObject();
        }
        else
            root = new JsonObject();

        // 兼容历史 key 分段差异（如 Y_Y_S_Y_POWER vs Y_YS_Y_POWER）：
        // 在同 prefix 下按“去掉下划线后的大写段”匹配，若已存在则复用其 baseKey。
        baseKey = ResolveExistingOrDefaultBaseKey(root, prefixUpper, powerClassName.Trim(), baseKey);
        titleKey = $"{baseKey}.title";
        descKey = $"{baseKey}.description";
        smartKey = $"{baseKey}.smartDescription";

        root[titleKey] = title ?? "";
        root[descKey] = description ?? "";
        root[smartKey] = smartDescription ?? "";

        File.WriteAllText(full, WriteJsonObjectIndented(root));
    }

    public static (string? title, string? description, string? smartDescription) TryReadPowerTexts(
        string path,
        string prefixUpper,
        string powerClassName)
    {
        if (string.IsNullOrWhiteSpace(path))
            return (null, null, null);
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return (null, null, null);

        try
        {
            var parsed = JsonNode.Parse(File.ReadAllText(full)) as JsonObject;
            if (parsed == null)
                return (null, null, null);
            var baseKey = BuildLocalizationKeyBase(prefixUpper, powerClassName);
            baseKey = ResolveExistingOrDefaultBaseKey(parsed, prefixUpper, powerClassName, baseKey);
            var titleKey = $"{baseKey}.title";
            var descKey = $"{baseKey}.description";
            var smartKey = $"{baseKey}.smartDescription";
            return (
                parsed[titleKey]?.GetValue<string>(),
                parsed[descKey]?.GetValue<string>(),
                parsed[smartKey]?.GetValue<string>()
            );
        }
        catch
        {
            return (null, null, null);
        }
    }

    private static string ResolveExistingOrDefaultBaseKey(
        JsonObject root,
        string prefixUpper,
        string powerClassName,
        string defaultBaseKey)
    {
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefixUpper)
            ? "POWER"
            : prefixUpper.Trim().ToUpperInvariant();
        var normalizedSeg = NormalizeSeg(CardLocalizationJsonMerger.ClassNameToLocalizationSegment(powerClassName));
        var prefixToken = normalizedPrefix + "-";

        foreach (var kv in root)
        {
            var k = kv.Key;
            if (string.IsNullOrWhiteSpace(k))
                continue;
            if (!k.EndsWith(".title", StringComparison.Ordinal) &&
                !k.EndsWith(".description", StringComparison.Ordinal) &&
                !k.EndsWith(".smartDescription", StringComparison.Ordinal))
                continue;

            var dot = k.LastIndexOf('.');
            if (dot <= 0)
                continue;
            var baseKey = k[..dot];
            if (!baseKey.StartsWith(prefixToken, StringComparison.OrdinalIgnoreCase))
                continue;

            var seg = baseKey.Substring(prefixToken.Length);
            if (NormalizeSeg(seg) == normalizedSeg)
                return baseKey;
        }

        return defaultBaseKey;
    }

    private static string NormalizeSeg(string seg) =>
        seg.Replace("_", "", StringComparison.Ordinal).Trim().ToUpperInvariant();

    private static string WriteJsonObjectIndented(JsonObject root)
    {
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms, JsonWriteWriterOptions))
        {
            root.WriteTo(writer);
        }
        return JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(Encoding.UTF8.GetString(ms.ToArray()));
    }
}

