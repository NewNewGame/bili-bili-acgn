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

    /// <summary>卡牌显示名称（与 localization 中 title 对应）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    /// <summary>卡牌描述正文（与 localization 中 description 对应）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

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

    /// <summary>生成 <c>CanonicalKeywords =&gt; [CustomKeyWords.xxx, ...]</c>，字段名与 <c>CustomKeyWords</c> 静态成员一致。</summary>
    [JsonPropertyName("canonicalKeywordFields")]
    public List<string> CanonicalKeywordFields { get; set; } = [];

    /// <summary>生成 <c>ExtraHoverTips =&gt; [HoverTipFactory.FromKeyword(CustomKeyWords.xxx), ...]</c>。</summary>
    [JsonPropertyName("extraHoverTipKeywordFields")]
    public List<string> ExtraHoverTipKeywordFields { get; set; } = [];

    [JsonPropertyName("dynamicVars")]
    public List<DynamicVarEntry> DynamicVars { get; set; } = [];

    /// <summary>打出后按顺序执行的 CardPlayAction（牌面效果编排）。</summary>
    [JsonPropertyName("cardPlayActions")]
    public List<CardPlayAction> CardPlayActions { get; set; } = [];

    /// <summary>升级时除动态变量 <c>upgradeValue</c> 外的额外效果，与 DynamicVars 升级顺序为：先全部 UpgradeValueBy，再按本列表顺序执行。</summary>
    [JsonPropertyName("upgradeEffects")]
    public List<UpgradeEffectEntry> UpgradeEffects { get; set; } = [];

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
