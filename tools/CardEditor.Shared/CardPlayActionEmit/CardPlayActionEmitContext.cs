using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 生成 <see cref="CardPlayAction"/> 对应 C# 片段时的上下文（缩进、动态变量表、卡牌目标类型等）。
/// 不引用游戏内 sts2 程序集，<see cref="PlayTargetType"/> 由 <see cref="CardDefinition.TargetType"/> 解析为编辑器 <see cref="TargetType"/>。
/// </summary>
public sealed class CardPlayActionEmitContext
{
    /// <summary>方法体内一行前的空白，默认 8 个空格（与 <c>OnPlay</c> 体对齐）。</summary>
    public string Indent { get; init; } = "        ";

    /// <summary>与 <see cref="CardDefinition.TargetType"/> 对应，生成代码中 <c>TargetType.XXX</c> 成员名与 <see cref="TargetType"/> 一致。</summary>
    public TargetType PlayTargetType { get; init; } = TargetType.AnyEnemy;

    /// <summary>可选：用于解析 Block 等的 <see cref="ValueProp"/>；为 null 时使用合理默认。</summary>
    public IReadOnlyList<DynamicVarEntry>? CanonicalVars { get; init; }

    /// <summary>从编辑器 <see cref="CardDefinition"/> 填充目标类型与 <see cref="CanonicalVars"/>。</summary>
    public static CardPlayActionEmitContext FromDefinition(CardDefinition def) =>
        new()
        {
            PlayTargetType = CardTargetTypeMapping.ParseFromDefinition(def.TargetType),
            CanonicalVars = def.DynamicVars
        };
}
