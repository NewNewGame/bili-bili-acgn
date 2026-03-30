using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 打出效果（<see cref="CardPlayAction"/>）模版：name 与添加到卡牌时的 <see cref="CardPlayAction.ActionType"/> 一致。
/// </summary>
public sealed class CardPlayActionTemplate
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
