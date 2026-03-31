using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>升级效果中「添加/移除关键字」下拉可选项（对应 <c>CustomKeyWords</c> 上的静态字段名）。</summary>
public sealed class KeywordOptionEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
