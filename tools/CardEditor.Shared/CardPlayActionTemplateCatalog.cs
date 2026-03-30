using CardEditor.Shared.Models;

namespace CardEditor.Shared;

/// <summary>内置打出效果模版（当 exe 旁无 <c>CardPlayActionTemplate.json</c> 或文件为空时使用）。</summary>
public static class CardPlayActionTemplateCatalog
{
    public static List<CardPlayActionTemplate> CloneDefaultTemplates() =>
        DefaultTemplates
            .Select(t => new CardPlayActionTemplate
            {
                Name = t.Name,
                Description = t.Description
            })
            .ToList();

    private static readonly IReadOnlyList<CardPlayActionTemplate> DefaultTemplates =
    [
        new() { Name = "DrawCards", Description = "抽牌" },
        new() { Name = "Damage", Description = "造成伤害" },
        new() { Name = "Block", Description = "获得格挡" },
        new() { Name = "Discard", Description = "弃牌" },
        new() { Name = "Exhaust", Description = "消耗牌" }
    ];
}
