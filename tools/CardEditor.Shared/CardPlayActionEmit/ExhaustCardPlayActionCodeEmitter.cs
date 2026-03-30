using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class ExhaustCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Exhaust";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner = $"{indent}// TODO: Exhaust {v} 张牌（按项目 API 替换为实际消耗指令）";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
