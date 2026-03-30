using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 施加 BUFF 打出效果代码生成器（占位）。
/// 具体生成规则由你后续手动实现。
/// </summary>
public sealed class BuffCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Buff";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var buff = string.IsNullOrWhiteSpace(action.BuffType) ? "<未指定BuffType>" : action.BuffType.Trim();
        return $"{indent}// TODO: Buff emitter not implemented yet. BuffType={buff}";
    }
}

