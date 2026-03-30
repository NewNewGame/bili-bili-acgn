using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>内置卡池列表（当 exe 旁无 <c>CardPool.json</c> 或文件为空时使用）。</summary>
public static class CardPoolCatalog
{
    public static List<CardPoolEntry> CloneDefaultPools() =>
        DefaultPools
            .Select(p => new CardPoolEntry { Name = p.Name, DisplayName = p.DisplayName })
            .ToList();

    private static readonly IReadOnlyList<CardPoolEntry> DefaultPools =
    [
        new() { Name = "ColorlessCardPool", DisplayName = "无色牌" },
        new() { Name = "CurseCardPool", DisplayName = "诅咒牌" },
        new() { Name = "StatusCardPool", DisplayName = "状态牌" }
    ];
}
