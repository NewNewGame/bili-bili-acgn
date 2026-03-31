using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public static class CardKeywordCatalog
{
    public static List<KeywordOptionEntry> CloneDefault() =>
    [
        new KeywordOptionEntry { Name = "YYSY", Notes = "有一说一", MemberPrefix = null },
        new KeywordOptionEntry { Name = "Anger", Notes = "愤怒", MemberPrefix = null },
        new KeywordOptionEntry { Name = "Exhaust", Notes = "消耗", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Ethereal", Notes = "虚无", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Innate", Notes = "固有", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Unplayable", Notes = "无法打出", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Retain", Notes = "保留", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Sly", Notes = "奇巧", MemberPrefix = "CardKeyword" },
        new KeywordOptionEntry { Name = "Eternal", Notes = "永恒", MemberPrefix = "CardKeyword" }
    ];
}
