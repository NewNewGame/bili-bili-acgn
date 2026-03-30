using CardEditor.Shared.Models;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 将单条 <see cref="CardPlayAction"/> 转为 <c>OnPlay</c> 内 C# 片段。具体种类由子类通过 <see cref="ActionTypeKey"/> 与 <see cref="CardPlayAction.ActionType"/> 对应。
/// </summary>
public abstract class CardPlayActionCodeEmitterBase
{
    /// <summary>与 <see cref="CardPlayAction.ActionType"/> 一致（如 Damage、DrawCards），注册表按忽略大小写匹配。</summary>
    public abstract string ActionTypeKey { get; }

    /// <summary>生成该打出效果对应的 C# 源码片段（不含方法声明）。可重写以统一加前后缀；默认调用 <see cref="GenerateCode"/>。</summary>
    public virtual string Emit(CardPlayAction action, CardPlayActionEmitContext context)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);
        return GenerateCode(action, context);
    }

    /// <summary>子类重写实现具体指令。</summary>
    protected abstract string GenerateCode(CardPlayAction action, CardPlayActionEmitContext context);
}
