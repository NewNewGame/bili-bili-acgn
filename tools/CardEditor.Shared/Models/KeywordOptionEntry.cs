using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 关键字选项：成员名 + 可选所属类/枚举前缀（未填则按 <c>CustomKeyWords</c> 处理）。
/// 内置游戏枚举可填 <c>CardKeyword</c>，成员如 <c>Exhaust</c> → 生成 <c>CardKeyword.Exhaust</c>。
/// </summary>
public sealed class KeywordOptionEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>成员前的类型前缀，如 <c>CustomKeyWords</c>、<c>CardKeyword</c>；空则视为 CustomKeyWords。</summary>
    [JsonPropertyName("memberPrefix")]
    public string? MemberPrefix { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>生成代码时使用的完整成员引用，如 <c>CustomKeyWords.YYSY</c>、<c>CardKeyword.Exhaust</c>。</summary>
    [JsonIgnore]
    public string QualifiedKey =>
        $"{(string.IsNullOrWhiteSpace(MemberPrefix) ? "CustomKeyWords" : MemberPrefix.Trim())}.{Name.Trim()}";

    /// <summary>用于下拉/列表显示：有 notes 时附带显示。</summary>
    [JsonIgnore]
    public string DisplayLabel
    {
        get
        {
            var q = QualifiedKey;
            var notes = Notes?.Trim();
            return string.IsNullOrEmpty(notes) ? q : $"{q} — {notes}";
        }
    }

    public static string FormatQualified(string name, string? memberPrefix)
    {
        var p = string.IsNullOrWhiteSpace(memberPrefix) ? "CustomKeyWords" : memberPrefix.Trim();
        var n = name?.Trim() ?? "";
        return $"{p}.{n}";
    }
}
