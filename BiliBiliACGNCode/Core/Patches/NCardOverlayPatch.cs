//****************** 代码文件申明 ***********************
//* 文件：NCardOverlayPatch
//* 作者：wheat
//* 创建时间：2026/04/17 10:51:00 星期一
//* 描述：修改卡牌的视效
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Nodes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(NCard))]
public static class NCardOverlayPatch
{
    private static string OverlayPath => "res://BiliBiliACGN/scenes/vfx/card/overlay/no_right_to_knight_me/vfx_ui_card_overlay_no_right_to_knight_me.tscn";
    private static Node OverlayContainer(NCard __instance) => __instance.GetNode<Node>("%OverlayContainer");
    [HarmonyPostfix]
    [HarmonyPatch("ReloadOverlay")]
    public static void ReloadOverlay_Postfix(NCard __instance)
    {   
        if(__instance.Model == null)
        {
            Log.Debug("[NCardOverlayPatch] ReloadOverlay：Model 为空，跳过。");
            return;
        }

        if(__instance.Model is NoRightToKnightMe){
            Log.Debug("[NCardOverlayPatch] ReloadOverlay：命中 NoRightToKnightMe，准备添加覆盖层。");

            var packed = PreloadManager.Cache.GetScene(OverlayPath);
            if (packed == null)
            {
                Log.Warn($"[NCardOverlayPatch] ReloadOverlay：覆盖层场景加载失败：{OverlayPath}");
                return;
            }

            // 获取容器
            var container = OverlayContainer(__instance);
            if(container == null)
            {
                Log.Warn("[NCardOverlayPatch] ReloadOverlay：未找到 OverlayContainer 节点。");
                return;
            }
            // 如果已经存在特效了，那就不再添加了
            foreach(var child in container.GetChildren())
            {
                if(child is SNNoRightToKnightMeOverlayVfx)
                {
                    Log.Debug("[NCardOverlayPatch] ReloadOverlay：覆盖层已存在，跳过。");
                    return;
                }
            }

            var overlay = packed.Instantiate<Control>(PackedScene.GenEditState.Disabled);
            if(overlay == null)
            {
                Log.Warn("[NCardOverlayPatch] ReloadOverlay：覆盖层实例化返回 null。");
                return;
            }


            container.AddChildSafely(overlay);
            Log.Debug("[NCardOverlayPatch] ReloadOverlay：覆盖层已添加。");
        }
    }
}