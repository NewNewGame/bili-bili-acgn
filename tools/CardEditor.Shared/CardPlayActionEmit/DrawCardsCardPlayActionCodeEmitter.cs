using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

public sealed class DrawCardsCardPlayActionCodeEmitter : CardPlayActionCodeEmitterBase
{
    public override string ActionTypeKey => "DrawCards";

    protected override string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context)
    {
        var indent = context.Indent;
        var v = CardPlayActionEmitSyntax.ValueExpression(action);
        var inner = $"{indent}await CardPileCmd.Draw(choiceContext, {v}, base.Owner);";
        return CardPlayActionEmitSyntax.WrapWithRepeatLoop(action, inner, indent);
    }
}
