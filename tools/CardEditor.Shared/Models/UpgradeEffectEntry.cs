using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>卡牌升级时除动态变量 <c>UpgradeValueBy</c> 外的额外效果（顺序与列表一致）。</summary>
public sealed class UpgradeEffectEntry
{
    /// <summary>
    /// <c>EnergyCostDelta</c>：调用 <c>base.EnergyCost.UpgradeBy(delta)</c>；<br/>
    /// <c>AddKeyword</c> / <c>RemoveKeyword</c>：<c>CustomKeyWords.{keywordField}</c>。
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "EnergyCostDelta";

    /// <summary>仅当 <see cref="Kind"/> 为 <c>EnergyCostDelta</c> 时有效（通常为 -1）。</summary>
    [JsonPropertyName("delta")]
    public int Delta { get; set; }

    /// <summary>仅当 <see cref="Kind"/> 为 <c>AddKeyword</c> / <c>RemoveKeyword</c> 时有效。</summary>
    [JsonPropertyName("keywordField")]
    public string? KeywordField { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
