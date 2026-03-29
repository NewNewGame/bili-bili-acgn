using System.Text.Json;
using System.Text.Json.Nodes;

namespace CardEditor.Shared;

/// <summary>
/// 将卡牌 title/description 合并到游戏内 <c>cards.json</c>（键格式 <c>BILIBILIACGN-{SEGMENT}.title</c>）。
/// </summary>
public static class CardLocalizationJsonMerger
{
    private const string KeyPrefix = "BILIBILIACGN-";

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>PascalCase 类名 → <c>TEST_CARD</c> 形式，与现有 localization 键一致。</summary>
    public static string ClassNameToLocalizationSegment(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return "CARD";
        var s = className.Trim();
        var sb = new System.Text.StringBuilder(s.Length * 2);
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (char.IsUpper(c) && i > 0)
                sb.Append('_');
            sb.Append(char.ToUpperInvariant(c));
        }
        return sb.ToString();
    }

    /// <summary>创建或更新对应键并写回文件。</summary>
    public static void MergeTitleAndDescription(string path, string className, string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("路径不能为空。", nameof(path));

        var full = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(full);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var segment = ClassNameToLocalizationSegment(className);
        var titleKey = $"{KeyPrefix}{segment}.title";
        var descKey = $"{KeyPrefix}{segment}.description";

        JsonObject root;
        if (File.Exists(full))
        {
            var text = File.ReadAllText(full);
            var parsed = JsonNode.Parse(text);
            root = parsed as JsonObject ?? new JsonObject();
        }
        else
            root = new JsonObject();

        root[titleKey] = title ?? "";
        root[descKey] = description ?? "";

        File.WriteAllText(full, root.ToJsonString(WriteOptions));
    }
}
