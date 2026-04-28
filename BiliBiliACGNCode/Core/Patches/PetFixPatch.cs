//****************** 代码文件申明 ***********************
//* PetFixPatch
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：宠物修复补丁
//*******************************************************

using System.Reflection;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(NCombatRoom))]
public static class PetFixPatch
{
    /// <summary>
    /// 读取 <c>NCombatRoom</c> 上绑定的 <see cref="Creature"/>（私有字段 <c>_creatureNodes</c>）。
    /// </summary>
    private static readonly Lazy<FieldInfo?> CreatureNodesField = new(() =>
        AccessTools.DeclaredField(typeof(NCombatRoom), "_creatureNodes"));
    /// <summary>
    /// 添加宠物后修复宠物血条
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="creature"></param>

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCombatRoom.AddCreature))]
    public static void AddCreature_Postfix(NCombatRoom __instance, Creature creature)
    {
        if(__instance == null) return;
        // 只处理宠物
        if (creature?.PetOwner == null) return;

        // 只处理本地玩家的宠物（避免远端玩家宠物）
        if (!LocalContext.IsMe(creature.PetOwner)) return;
        // 获取节点
        NCreature? nCreature = __instance.GetCreatureNode(creature);
        NCreature? creatureNode = __instance.GetCreatureNode(creature.PetOwner.Creature);
        // 如果节点不存在，则返回
        if(nCreature == null || creatureNode == null) return;
        // 如果不是一果，需要把一果的血条重新打开（原版会全部关闭）
        if (creature.Monster is not Itsuka) {
            Player player = creatureNode.Entity.Player;
            // 反射获取_creatureNodes
            List<NCreature> list = CreatureNodesField.Value?.GetValue(__instance) as List<NCreature>;
            list = list?.Where(node => node.Entity.PetOwner == player && (!(node.Entity.Monster is Osty) || !LocalContext.IsMe(player))).ToList();
            if(list == null) return;
            // 遍历所有宠物，如果宠物是一果，则打开血条
            for(int i = 0; i < list.Count; i++) {
                NCreature node = list[i];
                if(node.Entity.Monster is Itsuka){
                    // 打开交互
                    node.ToggleIsInteractable(true);
                    node.Position = new Godot.Vector2(creatureNode.Position.X + 240f, creatureNode.Position.Y + 15f);
                }
            }
            return;
        }
        // 强制打开交互（会显示血条 + 可点选 + 进手柄导航）
        nCreature.ToggleIsInteractable(true);
        nCreature.Position = new Godot.Vector2(creatureNode.Position.X + 240f, creatureNode.Position.Y + 15f);
    }
    
}