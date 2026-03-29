using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 卡牌编辑器持久化到磁盘的根对象（与模组运行时代码解耦，仅作数据交换）。
/// </summary>
public sealed class CardDefinition
{
    [JsonPropertyName("schemaVersion")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("className")]
    public string ClassName { get; set; } = "";

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = "BiliBiliACGN.BiliBiliACGNCode.Cards";

    [JsonPropertyName("energyCost")]
    public int EnergyCost { get; set; }

    [JsonPropertyName("cardType")]
    public string CardType { get; set; } = "Attack";

    [JsonPropertyName("rarity")]
    public string Rarity { get; set; } = "Common";

    [JsonPropertyName("targetType")]
    public string TargetType { get; set; } = "AnyEnemy";

    [JsonPropertyName("showInCardLibrary")]
    public bool ShowInCardLibrary { get; set; } = true;

    [JsonPropertyName("poolTypeName")]
    public string PoolTypeName { get; set; } = "ColorlessCardPool";

    [JsonPropertyName("dynamicVars")]
    public List<DynamicVarEntry> DynamicVars { get; set; } = [];

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
