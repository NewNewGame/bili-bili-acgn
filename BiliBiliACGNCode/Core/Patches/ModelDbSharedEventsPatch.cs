using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch]
public static class ModelDbSharedEventsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("MegaCrit.Sts2.Core.Models.ModelDb", "get_AllSharedEvents")]
    public static void GetAllSharedEvents_Postfix(ref IEnumerable<EventModel> __result)
    {
        var extra = Core.EventRegister.GetExtraSharedEvents();
        __result = (__result ?? []).Concat(extra).Distinct();
    }
}

