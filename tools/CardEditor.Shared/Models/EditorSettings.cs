using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 卡牌编辑器全局设置（与单张卡牌 JSON 无关）。
/// </summary>
public sealed class EditorSettings
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    /// <summary>生成卡牌类时使用的默认命名空间。</summary>
    [JsonPropertyName("defaultNamespace")]
    public string DefaultNamespace { get; set; } = "BiliBiliACGN.BiliBiliACGNCode.Cards";

    /// <summary>打开/保存对话框的默认目录；空则使用「文档」。</summary>
    [JsonPropertyName("defaultSaveDirectory")]
    public string DefaultSaveDirectory { get; set; } = "";

    /// <summary>生成卡牌 C# 脚本时的默认输出目录（如 BiliBiliACGNCode/Cards）；空表示未指定，由生成器决定。</summary>
    [JsonPropertyName("defaultCardScriptOutputDirectory")]
    public string DefaultCardScriptOutputDirectory { get; set; } = "";

    /// <summary>游戏内卡牌文案 JSON（如 localization/zhs/cards.json）的完整路径；空表示未配置。</summary>
    [JsonPropertyName("cardLocalizationJsonPath")]
    public string CardLocalizationJsonPath { get; set; } = "";

    /// <summary>卡池类型名下拉选项（对应 [Pool(typeof(...))] 中的类型名）。</summary>
    [JsonPropertyName("poolTypeOptions")]
    public List<string> PoolTypeOptions { get; set; } =
    [
        "ColorlessCardPool"
    ];
}
