using CardEditor.Shared.Models;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// <see cref="CardDefinition.TargetType"/> 字符串与运行时 <see cref="TargetType"/> 的对应（与 <see cref="CardCodeGenerator"/> 中名称一致）。
/// </summary>
public static class CardTargetTypeMapping
{
    /// <summary>将 JSON / <see cref="CardDefinition.TargetType"/> 转为 sts2 枚举。</summary>
    public static TargetType FromDefinitionString(string? targetType)
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
