using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class DiscardCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "Discard";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner = $"{indent}// TODO: Discard {v} 张牌（按项目 API 替换为实际弃牌指令）";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
