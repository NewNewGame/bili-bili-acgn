using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 卡池选项：写入卡牌 JSON / 生成代码时使用 <see cref="Name"/>；界面下拉优先展示 <see cref="DisplayName"/>（若有）。
/// </summary>
public sealed class CardPoolEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>界面显示名；空则界面显示 <see cref="Name"/>。</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>下拉框展示用：有显示名用显示名，否则用类型名。</summary>
    [JsonIgnore]
    public string DisplayLabel => string.IsNullOrWhiteSpace(DisplayName) ? Name : DisplayName.Trim();
}
