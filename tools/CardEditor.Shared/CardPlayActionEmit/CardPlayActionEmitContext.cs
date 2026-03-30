using CardEditor.Shared.Models;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 生成 <see cref="CardPlayAction"/> 对应 C# 片段时的上下文（缩进、动态变量表、卡牌目标类型等）。
/// </summary>
public sealed class CardPlayActionEmitContext
{
    /// <summary>方法体内一行前的空白，默认 8 个空格（与 <c>OnPlay</c> 体对齐）。</summary>
    public string Indent { get; init; } = "        ";

    /// <summary>
    /// 运行时 <see cref="TargetType"/>。从 JSON 构造时请使用 <see cref="CardTargetTypeMapping.FromDefinitionString"/> 传入
    /// <see cref="CardDefinition.TargetType"/> 字符串。
    /// </summary>
    public TargetType TargetType { get; init; } = TargetType.AnyEnemy;

    /// <summary>可选：用于解析 Block 等的 <see cref="ValueProp"/>；为 null 时使用合理默认。</summary>
    public IReadOnlyList<DynamicVarEntry>? CanonicalVars { get; init; }

    /// <summary>从编辑器 <see cref="CardDefinition"/> 填充 <see cref="TargetType"/> 与 <see cref="CanonicalVars"/>。</summary>
    public static CardPlayActionEmitContext FromDefinition(CardDefinition def) =>
        new()
        {
            TargetType = CardTargetTypeMapping.FromDefinitionString(def.TargetType),
            CanonicalVars = def.DynamicVars
        };
}
