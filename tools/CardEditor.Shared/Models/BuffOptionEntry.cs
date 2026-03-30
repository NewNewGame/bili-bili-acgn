using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>打出效果中可选的 BUFF 类型（名称 + 备注）。</summary>
public sealed class BuffOptionEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

