//****************** 代码文件申明 ***********************
//* 文件：ArchaicToothPatch
//* 作者：wheat
//* 创建时间：2026/04/04
//* 描述：Harmony Postfix，卡牌升级配置
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("get_TranscendenceUpgrades")]
    public static void GetTranscendenceUpgrades_Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        // 卡牌升级配置
        // 如果原版 __result 中没有PowerlessAngry，则添加Meditation
        if(__result.ContainsKey(ModelDb.Card<PowerlessAngry>().Id))
            return;
        __result[ModelDb.Card<PowerlessAngry>().Id] = ModelDb.Card<Meditation>();
    }
}