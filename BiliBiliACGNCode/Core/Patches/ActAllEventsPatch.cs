using System.Reflection;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch]
public static class ActAllEventsPatch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var modelAsm = typeof(ActModel).Assembly;
        foreach (var t in modelAsm.GetTypes())
        {
            if (t.IsAbstract || !typeof(ActModel).IsAssignableFrom(t))
                continue;
            var getter = t.GetProperty(nameof(ActModel.AllEvents), BindingFlags.Instance | BindingFlags.Public)?.GetMethod;
            if (getter != null && !getter.IsAbstract)
                yield return getter;
        }
    }

    [HarmonyPostfix]
    public static void GetAllEvents_Postfix(ActModel __instance, ref IEnumerable<EventModel> __result)
    {
        var extra = Core.EventRegister.GetExtraEventsForAct(__instance.GetType());
        __result = (__result ?? []).Concat(extra).Distinct();
    }
}

