//****************** 代码文件申明 ***********************
//* 文件：CustomVfxCmd
//* 作者：wheat
//* 创建时间：2026/04/12
//* 描述：自定义VFX命令
//*******************************************************

using System.Linq;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Commands;

public static class CustomVfxCmd
{
    public static readonly string BerserkPath = "vfx/vfx_berserk";
    public static readonly string InfiniteBullnessPath = "vfx/vfx_infinite_bullness";
    public static readonly string NoRightToKnightMePath = "vfx/vfx_no_right_to_knight_me";
    /// <summary>
    /// 添加指定目标的VFX
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public static Node2D? AddVfx(Creature target, string path){
        if(target == null) return null;
        if (!TestMode.IsOn && NCombatRoom.Instance != null)
        {
            NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
            if (creatureNode != null)
            {
                string scenePath = SceneHelper.GetScenePath(path);
                Node2D node2D = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
                creatureNode.AddChildSafely(node2D);
                node2D.GlobalPosition = creatureNode.GlobalPosition;
                return node2D;
            }
        }
        return null;
    }
    /// <summary>
    /// 添加指定目标的VFX
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public static Node2D? AddVfxOnCenter(Creature target, string path){
        if(target == null) return null;
        if (!TestMode.IsOn && NCombatRoom.Instance != null)
        {
            NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
            if (creatureNode != null)
            {
                string scenePath = SceneHelper.GetScenePath(path);
                Node2D node2D = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
                creatureNode.AddChildSafely(node2D);
                node2D.GlobalPosition = creatureNode.VfxSpawnPosition;
                return node2D;
            }
        }
        return null;
    }
    /// <summary>
    /// 删除指定目标的VFX
    /// </summary>
    public static void RemoveVfx<T>(Creature target) where T : Node2D{
        if(target == null) return;
        if (!TestMode.IsOn && NCombatRoom.Instance != null)
        {
            NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
            if (creatureNode != null)
            {
                RemoveVfx<T>(creatureNode);
            }
        }
    }
    /// <summary>
    /// 删除指定位置的VFX
    /// </summary>
    public static void RemoveVfx<T>(NCreature parent) where T : Node2D{
        if(parent == null) return;
        if (!TestMode.IsOn && NCombatRoom.Instance != null)
        {
            // 遍历所有子节点， 找到类型为T的节点，然后删除
            foreach(var child in parent.GetChildren()){
                if(child is T node){
                    node.QueueFreeSafely();
                    return;
                }
            }
        }
    }
}