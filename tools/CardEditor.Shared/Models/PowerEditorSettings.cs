using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// Powers 文本信息编辑器的独立设置（与总编辑器 settings.json 分离，保存在 exe 同目录）。
/// </summary>
public sealed class PowerEditorSettings
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    /// <summary>打开 Power.cs 对话框的默认目录；空则尝试猜测仓库路径。</summary>
    [JsonPropertyName("defaultPowersCsDirectory")]
    public string DefaultPowersCsDirectory { get; set; } = "";

    /// <summary>能力文案 JSON（powers.json）完整路径；空表示未配置。</summary>
    [JsonPropertyName("powerLocalizationJsonPath")]
    public string PowerLocalizationJsonPath { get; set; } = "";

    /// <summary>本地化 key 前缀（命名空间前缀），例如 BILIBILIACGN。</summary>
    [JsonPropertyName("powerLocalizationNamespacePrefix")]
    public string PowerLocalizationNamespacePrefix { get; set; } = "BILIBILIACGN";
}

