using System.Text;
using System.Text.Json.Nodes;

namespace CardEditor.Shared;

/// <summary>
/// 将 keyword 的 title/description 合并到游戏内 <c>card_keywords.json</c>。
/// 键为 <c>{prefix}-{keyword}.title</c> / <c>.description</c>。
/// </summary>
public static class KeywordLocalizationJsonMerger
{
    private static readonly System.Text.Json.JsonWriterOptions JsonWriteWriterOptions = new()
    {
        Indented = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    private static string NormalizeSeg(string seg) =>
        seg.Replace("_", "", StringComparison.Ordinal).Trim().ToUpperInvariant();

    private static string ResolveExistingOrDefaultBaseKey(JsonObject root, string prefixUpper, string keyword, string defaultBaseKey)
    {
        var normalizedPrefix = string.IsNullOrWhiteSpace(prefixUpper) ? "KEYWORD" : prefixUpper.Trim().ToUpperInvariant();
        var prefixToken = normalizedPrefix + "-";
        var normalizedKeyword = NormalizeSeg(keyword);

        foreach (var kv in root)
        {
            var k = kv.Key;
            if (string.IsNullOrWhiteSpace(k))
                continue;
            if (!k.EndsWith(".title", StringComparison.Ordinal) && !k.EndsWith(".description", StringComparison.Ordinal))
                continue;
            var dot = k.LastIndexOf('.');
            if (dot <= 0)
                continue;
            var baseKey = k[..dot];
            if (!baseKey.StartsWith(prefixToken, StringComparison.OrdinalIgnoreCase))
                continue;
            var seg = baseKey.Substring(prefixToken.Length);
            if (NormalizeSeg(seg) == normalizedKeyword)
                return baseKey;
        }

        return defaultBaseKey;
    }

    public static void MergeKeywordTexts(string path, string prefixUpper, string keyword, string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("路径不能为空。", nameof(path));
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("keyword 不能为空。", nameof(keyword));

        var full = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        JsonObject root;
        if (File.Exists(full))
        {
            var parsed = JsonNode.Parse(File.ReadAllText(full));
            root = parsed as JsonObject ?? new JsonObject();
        }
        else
            root = new JsonObject();

        var p = string.IsNullOrWhiteSpace(prefixUpper) ? "KEYWORD" : prefixUpper.Trim().ToUpperInvariant();
        var seg = keyword.Trim();
        var defaultBaseKey = $"{p}-{seg}";
        var baseKey = ResolveExistingOrDefaultBaseKey(root, p, seg, defaultBaseKey);

        root[$"{baseKey}.title"] = title ?? "";
        root[$"{baseKey}.description"] = description ?? "";

        File.WriteAllText(full, WriteJsonObjectIndented(root));
    }

    public static (string? title, string? description) TryReadKeywordTexts(string path, string prefixUpper, string keyword)
    {
        if (string.IsNullOrWhiteSpace(path))
            return (null, null);
        if (string.IsNullOrWhiteSpace(keyword))
            return (null, null);
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return (null, null);

        try
        {
            var root = JsonNode.Parse(File.ReadAllText(full)) as JsonObject;
            if (root == null)
                return (null, null);
            var p = string.IsNullOrWhiteSpace(prefixUpper) ? "KEYWORD" : prefixUpper.Trim().ToUpperInvariant();
            var seg = keyword.Trim();
            var defaultBaseKey = $"{p}-{seg}";
            var baseKey = ResolveExistingOrDefaultBaseKey(root, p, seg, defaultBaseKey);
            return (
                root[$"{baseKey}.title"]?.GetValue<string>(),
                root[$"{baseKey}.description"]?.GetValue<string>()
            );
        }
        catch
        {
            return (null, null);
        }
    }

    private static string WriteJsonObjectIndented(JsonObject root)
    {
        using var ms = new MemoryStream();
        using (var writer = new System.Text.Json.Utf8JsonWriter(ms, JsonWriteWriterOptions))
        {
            root.WriteTo(writer);
        }
        return JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(Encoding.UTF8.GetString(ms.ToArray()));
    }
}

