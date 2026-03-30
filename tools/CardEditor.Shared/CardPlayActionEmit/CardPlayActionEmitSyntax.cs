using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 从 <see cref="CardPlayAction"/> 的绑定字段生成 C# 表达式与重复包装。
/// </summary>
public static class CardPlayActionEmitSyntax
{
    public static bool IsLiteralBinding(string? binding) =>
        string.IsNullOrWhiteSpace(binding) ||
        string.Equals(binding.Trim(), "literal", StringComparison.OrdinalIgnoreCase);

    public static string FormatDecimalLiteral(decimal d) =>
        d.ToString("0", CultureInfo.InvariantCulture) + "m";

    public static string ValueExpression(CardPlayAction action)
    {
        if (IsLiteralBinding(action.ValueBinding))
            return FormatDecimalLiteral(action.Value);
        return DynamicVarBaseValueExpression(action.ValueBinding!.Trim());
    }

    public static string RepeatCountExpression(CardPlayAction action)
    {
        if (IsLiteralBinding(action.RepeatCountBinding))
            return ((int)action.RepeatCountValue).ToString(CultureInfo.InvariantCulture);
        return DynamicVarBaseValueExpression(action.RepeatCountBinding!.Trim());
    }

    /// <summary>访问与 kind 对应的动态变量最终基值（与编辑器预览一致取 CanonicalVars 语义）。</summary>
    public static string DynamicVarBaseValueExpression(string kindTrimmed)
    {
        var k = kindTrimmed;
        if (k.Equals("Damage", StringComparison.OrdinalIgnoreCase))
            return "base.DynamicVars.Damage.BaseValue";
        if (k.Equals("Block", StringComparison.OrdinalIgnoreCase))
            return "base.DynamicVars.Block.BaseValue";
        if (k.Equals("Cards", StringComparison.OrdinalIgnoreCase))
            return "base.DynamicVars.Cards.BaseValue";
        var escaped = k.Replace("\\", "\\\\").Replace("\"", "\\\"");
        return $"base.DynamicVars[\"{escaped}\"].BaseValue";
    }

    /// <summary>
    /// 若重复次数为 1 次且为 literal，不包循环；否则用 <c>for</c> 包裹 <paramref name="innerBody"/>。
    /// 各 Emitter 按需调用；若需结合 <see cref="CardPlayActionEmitContext.TargetType"/> 等自定义重复结构，请在 <c>GenerateCode</c> 内自行编写循环，勿强制在末尾调用本方法。
    /// </summary>
    public static string WrapWithRepeatLoop(CardPlayAction action, string innerBody, string indent)
    {
        var literalRepeat = IsLiteralBinding(action.RepeatCountBinding);
        if (literalRepeat && action.RepeatCountValue <= 1m)
            return innerBody;

        var n = RepeatCountExpression(action);
        var sb = new StringBuilder();
        sb.AppendLine($"{indent}for (var __playIdx = 0; __playIdx < {n}; __playIdx++)");
        sb.AppendLine($"{indent}{{");
        foreach (var line in innerBody.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            if (line.Length == 0)
                continue;
            sb.AppendLine("    " + line);
        }
        sb.Append($"{indent}}}");
        return sb.ToString();
    }

    /// <summary>生成源码中的 <c>ValueProp</c> 表达式。</summary>
    public static string FormatValuePropForEmit(ValueProp p, string kindLower)
    {
        if (p == ValueProp.None)
        {
            return kindLower switch
            {
                "damage" => "ValueProp.Move",
                "block" => "ValueProp.Unpowered",
                _ => "ValueProp.None"
            };
        }
        var parts = new List<string>();
        if (p.HasFlag(ValueProp.Move)) parts.Add("ValueProp.Move");
        if (p.HasFlag(ValueProp.Unpowered)) parts.Add("ValueProp.Unpowered");
        if (p.HasFlag(ValueProp.Unblockable)) parts.Add("ValueProp.Unblockable");
        if (p.HasFlag(ValueProp.SkipHurtAnim)) parts.Add("ValueProp.SkipHurtAnim");
        return parts.Count == 0 ? "ValueProp.None" : string.Join(" | ", parts);
    }
}
