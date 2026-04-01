using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 在游戏核心初始化完成后再注册事件，避免 ModInitializer 过早访问 ModelDb 导致 KeyNotFound（如 ACT.OVERGROWTH）。
/// </summary>
[HarmonyPatch]
public static class OneTimeInitializationPatch
{
    private static bool _eventsRegistered;

    [HarmonyPostfix]
    [HarmonyPatch("MegaCrit.Sts2.Core.Helpers.OneTimeInitialization", "ExecuteEssential")]
    public static void ExecuteEssential_Postfix()
    {
        if (_eventsRegistered)
            return;
        _eventsRegistered = true;
        try
        {
            Core.EventRegister.RegisterEvents();
            Log.Debug("Mod Events initialized (deferred).");
        }
        catch (Exception ex)
        {
            Log.Error($"Mod Events init failed: {ex}");
        }
    }
}

