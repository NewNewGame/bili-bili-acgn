//****************** 代码文件申明 ***********************
//* PetFixPatch
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：宠物修复补丁
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(NCombatRoom))]
public static class Patch_NCombatRoom_AddCreature_AllowItsukaInteract
{
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatRoom.AddCreature))]
    public static void AddCreature_Postfix(NCombatRoom __instance, Creature creature)
    {
        // 只处理宠物
        if (creature?.PetOwner == null) return;

        // 只处理本地玩家的宠物（避免远端玩家宠物）
        if (!LocalContext.IsMe(creature.PetOwner)) return;

        // 只处理你的宠物类型
        if (creature.Monster is not Itsuka) return;

        // 拿到表现节点
        var node = __instance.GetCreatureNode(creature);
        if (node == null) return;

        // 强制打开交互（会显示血条 + 可点选 + 进手柄导航）
        node.ToggleIsInteractable(true);
    }
}