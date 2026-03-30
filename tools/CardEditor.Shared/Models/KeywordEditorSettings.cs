using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>Keywords（card_keywords.json）编辑器独立设置（保存在 exe 同目录）。</summary>
public sealed class KeywordEditorSettings
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    /// <summary>keywords.json 完整路径；空表示未配置。</summary>
    [JsonPropertyName("keywordLocalizationJsonPath")]
    public string KeywordLocalizationJsonPath { get; set; } = "";

    /// <summary>本地化 key 前缀（命名空间前缀），例如 BILIBILIACGN。</summary>
    [JsonPropertyName("keywordLocalizationNamespacePrefix")]
    public string KeywordLocalizationNamespacePrefix { get; set; } = "BILIBILIACGN";
}

