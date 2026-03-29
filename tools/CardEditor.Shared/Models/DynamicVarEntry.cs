using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

public sealed class DynamicVarEntry
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "Damage";

    [JsonPropertyName("baseValue")]
    public decimal BaseValue { get; set; }

    [JsonPropertyName("valueProp")]
    public string? ValueProp { get; set; }

    [JsonPropertyName("customKey")]
    public string? CustomKey { get; set; }
}
