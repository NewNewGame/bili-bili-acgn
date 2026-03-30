using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 按 <see cref="CardPlayActionCodeEmitterBase.ActionTypeKey"/> 查找打出效果代码生成器。
/// </summary>
public static class CardPlayActionCodeEmitterRegistry
{
    private static readonly CardPlayActionCodeEmitterBase[] AllArray =
    [
        new DamageCardPlayActionCodeEmitter(),
        new BuffCardPlayActionCodeEmitter(),
        new BlockCardPlayActionCodeEmitter(),
        new DrawCardsCardPlayActionCodeEmitter(),
        new DiscardCardPlayActionCodeEmitter(),
        new ExhaustCardPlayActionCodeEmitter()
    ];

    private static readonly Dictionary<string, CardPlayActionCodeEmitterBase> ByKey =
        AllArray.ToDictionary(e => e.ActionTypeKey, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<CardPlayActionCodeEmitterBase> All => AllArray;

    public static bool TryGet(string actionType, [NotNullWhen(true)] out CardPlayActionCodeEmitterBase? emitter) =>
        ByKey.TryGetValue(actionType.Trim(), out emitter);
}
