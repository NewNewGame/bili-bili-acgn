using System.Text.Json;
using System.Text.Json.Serialization;
using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public sealed class CardPoolFile
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("pools")]
    public List<CardPoolEntry> Pools { get; set; } = [];
}

/// <summary>读写 exe 同目录下的 <c>CardPool.json</c>（与 AppData 中 settings.json 分离）。</summary>
public static class CardPoolJson
{
    public const string FileName = "CardPool.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Encoder = JsonUnicodeEncoder.ForWritableJson
    };

    /// <summary>与 CardEditorGui.exe 同目录。</summary>
    public static string GetDefaultFilePath() =>
        Path.Combine(AppContext.BaseDirectory, FileName);

    public static string Serialize(CardPoolFile file) =>
        JsonUnicodeEncoder.ExpandJsonUnicodeEscapes(JsonSerializer.Serialize(file, Options));

    public static CardPoolFile Deserialize(string json) =>
        JsonSerializer.Deserialize<CardPoolFile>(json, Options)
        ?? new CardPoolFile();

    public static void SaveToFile(List<CardPoolEntry> pools, string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var file = new CardPoolFile { Pools = pools };
        File.WriteAllText(path, Serialize(file));
    }

    public static List<CardPoolEntry> LoadFromFile(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
            return MigrateFromLegacySettingsOrDefaults();
        try
        {
            var f = Deserialize(File.ReadAllText(full));
            if (f.Pools == null || f.Pools.Count == 0)
                return MigrateFromLegacySettingsOrDefaults();
            return f.Pools;
        }
        catch
        {
            return MigrateFromLegacySettingsOrDefaults();
        }
    }

    /// <summary>首次使用：若旧版 settings.json 里仍有 poolTypeOptions，则迁移；否则用内置默认。</summary>
    private static List<CardPoolEntry> MigrateFromLegacySettingsOrDefaults()
    {
        try
        {
            var settingsPath = EditorSettingsJson.GetDefaultSettingsPath();
            if (File.Exists(settingsPath))
            {
                var s = EditorSettingsJson.Deserialize(File.ReadAllText(settingsPath));
                if (s.PoolTypeOptions is { Count: > 0 })
                {
                    return s.PoolTypeOptions
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.Ordinal)
                        .Select(name => new CardPoolEntry { Name = name, DisplayName = null })
                        .ToList();
                }
            }
        }
        catch
        {
            // ignore
        }

        return CardPoolCatalog.CloneDefaultPools();
    }

    public static List<CardPoolEntry> LoadOrCreateDefault() =>
        LoadFromFile(GetDefaultFilePath());

    public static void SaveDefault(List<CardPoolEntry> pools) =>
        SaveToFile(pools, GetDefaultFilePath());
}
