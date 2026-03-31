using CardEditor.Shared.Models;

namespace CardEditor.Shared;

public static class CardKeywordCatalog
{
    public static List<KeywordOptionEntry> CloneDefault() =>
    [
        new KeywordOptionEntry { Name = "YYSY", Notes = "有一说一" },
        new KeywordOptionEntry { Name = "Anger", Notes = "愤怒" }
    ];
}
