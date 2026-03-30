using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// <see cref="CardDefinition.TargetType"/> 字符串 → 编辑器 <see cref="TargetType"/>（与 <see cref="CardCodeGenerator"/> 中 MapTargetType 一致）。
/// </summary>
public static class CardTargetTypeMapping
{
    /// <summary>将 JSON / 编辑器中的 targetType 解析为 <see cref="TargetType"/>。</summary>
    public static TargetType ParseFromDefinition(string? targetType)
    {
        var t = (targetType ?? "AnyEnemy").Trim();
        return t switch
        {
            "None" => TargetType.None,
            "Self" => TargetType.Self,
            "AnyEnemy" => TargetType.AnyEnemy,
            "AllEnemies" => TargetType.AllEnemies,
            "RandomEnemy" => TargetType.RandomEnemy,
            "AnyPlayer" => TargetType.AnyPlayer,
            "AnyAlly" => TargetType.AnyAlly,
            "AllAllies" => TargetType.AllAllies,
            "TargetedNoCreature" => TargetType.TargetedNoCreature,
            "Osty" => TargetType.Osty,
            "Player" => TargetType.AnyPlayer,
            _ => TargetType.AnyEnemy
        };
    }
}
