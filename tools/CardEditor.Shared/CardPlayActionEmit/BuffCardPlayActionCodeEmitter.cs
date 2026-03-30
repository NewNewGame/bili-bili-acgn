using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 施加 BUFF 打出效果代码生成器。
/// 参考手写卡：先触发施法动画，再 <c>PowerCmd.Apply&lt;TPower&gt;(...)</c>。
/// </summary>
public sealed class BuffCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Buff";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var buff = action.BuffType?.Trim();
        if (string.IsNullOrWhiteSpace(buff))
            return $"{indent}// TODO: BuffType 未指定，无法生成 PowerCmd.Apply<TPower>。";

        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner =
            $"{indent}await CreatureCmd.TriggerAnim(base.Owner.Creature, \"Cast\", base.Owner.Character.CastAnimDelay);\n" +
            $"{indent}await PowerCmd.Apply<{buff}>(base.Owner.Creature, {v}, base.Owner.Creature, this);";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}

