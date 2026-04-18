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
        if(__instance == null) return;
        // 获取容器
        var container = OverlayContainer(__instance);
        if(container == null) return;

        // 清空容器
        int cnt = container.GetChildCount();
        for(int i = cnt-1; i >= 0; i--)
        {
            var child = container.GetChild(i);
            if(child is SNOverlayVfxBase)
            {
                container.RemoveChildSafely(child);
                child.QueueFreeSafely();
            }
        }
        // 获取模型
        if(__instance.Model == null)
        {
            return;
        }

        if(__instance.Model is NoRightToKnightMe){
            // 获取特效Prefab
            var packed = PreloadManager.Cache.GetScene(OverlayPath);
            if (packed == null)
            {
                return;
            }

            // 实例化特效
            var overlay = packed.Instantiate<Control>(PackedScene.GenEditState.Disabled);
            if(overlay == null)
            {
                return;
            }

            container.AddChildSafely(overlay);
        }
    }
}