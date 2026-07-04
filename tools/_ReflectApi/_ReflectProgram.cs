using System.Linq;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

Console.WriteLine("=== CardCreationOptions ctors ===");
foreach (var ctor in typeof(CardCreationOptions).GetConstructors())
{
    var ps = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
    Console.WriteLine($"({ps})");
}

Console.WriteLine("=== CardCreationOptions static ===");
foreach (var m in typeof(CardCreationOptions).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(m => m.DeclaringType == typeof(CardCreationOptions)))
{
    var ps = string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
    Console.WriteLine($"{m.ReturnType.Name} {m.Name}({ps})");
}

var syncType = typeof(CardReward).Assembly.GetType("MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceSynchronizer");
Console.WriteLine($"Synchronizer nullable ctor param: {typeof(CardReward).GetConstructors()[1].GetParameters()[4].HasDefaultValue} default={typeof(CardReward).GetConstructors()[1].GetParameters()[4].DefaultValue}");
