using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class CardKeywordOptionsFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("options")]
    public List<KeywordOptionEntry> Options { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>CardKeywordOptions.json</c>（升级效果关键字下拉）。</summary>
public static class CardKeywordOptionsJson
{
    public const string FileName = "CardKeywordOptions.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    public static string GetDefaultFilePath() =>
        ExeBundledSettingsJson.GetDefaultFilePath();

    public static string Serialize(CardKeywordOptionsFile file) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(file, Options));

    public static CardKeywordOptionsFile Deserialize(string json) =>
        JsonSerializer.Deserialize<CardKeywordOptionsFile>(json, Options) ?? new CardKeywordOptionsFile();

    internal static CardKeywordOptionsFile DeserializeFile(string json) => Deserialize(json);

    public static List<KeywordOptionEntry> LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return [];
        try
        {
            var f = Deserialize(File.ReadAllText(full));
            return f.Options ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static void SaveToFile(List<KeywordOptionEntry> options, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(new CardKeywordOptionsFile { Options = options }));
    }

    public static List<KeywordOptionEntry> LoadOrCreateDefault()
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        var merged = MergeWithCatalog(root.CardKeywordOptions ?? []);
        // 只要能补齐默认项，就回写一次，避免用户每次都“缺内置项”
        if (!SequenceQualifiedEquals(root.CardKeywordOptions, merged))
        {
            root.CardKeywordOptions = merged;
            ExeBundledSettingsJson.SaveDefault(root);
        }
        return merged;
    }

    private static bool SequenceQualifiedEquals(List<KeywordOptionEntry>? a, List<KeywordOptionEntry> b)
    {
        if (a == null)
            return b.Count == 0;
        if (a.Count != b.Count)
            return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (!string.Equals(a[i].QualifiedKey, b[i].QualifiedKey, StringComparison.Ordinal))
                return false;
            if (!string.Equals(a[i].Notes, b[i].Notes, StringComparison.Ordinal))
                return false;
        }
        return true;
    }

    private static List<KeywordOptionEntry> MergeWithCatalog(List<KeywordOptionEntry> existing)
    {
        var merged = new List<KeywordOptionEntry>(existing.Count + 16);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var e in existing)
        {
            var name = e.Name?.Trim() ?? "";
            if (name.Length == 0)
                continue;
            var prefix = string.IsNullOrWhiteSpace(e.MemberPrefix) ? null : e.MemberPrefix.Trim();
            var normalized = new KeywordOptionEntry { Name = name, MemberPrefix = prefix, Notes = e.Notes };
            var key = normalized.QualifiedKey;
            if (seen.Add(key))
                merged.Add(normalized);
        }

        foreach (var d in CloneDefaultOptions())
        {
            var key = d.QualifiedKey;
            if (seen.Add(key))
                merged.Add(d);
        }

        return merged;
    }

    public static List<KeywordOptionEntry> CloneDefaultOptions() =>
        CardKeywordCatalog.CloneDefault()
            .Select(k => new KeywordOptionEntry { Name = k.Name, Notes = k.Notes, MemberPrefix = k.MemberPrefix })
            .ToList();

    public static void SaveDefault(List<KeywordOptionEntry> options)
    {
        var root = ExeBundledSettingsJson.LoadOrCreateDefault();
        root.CardKeywordOptions = options;
        ExeBundledSettingsJson.SaveDefault(root);
    }
}
