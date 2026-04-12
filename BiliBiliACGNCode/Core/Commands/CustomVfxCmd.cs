//****************** 代码文件申明 ***********************
//* 文件：CustomVfxCmd
//* 作者：wheat
//* 创建时间：2026/04/12
//* 描述：自定义VFX命令
//*******************************************************

using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Commands;

public static class CustomVfxCmd
{
    public static readonly string BerserkPath = "vfx/vfx_berserk";
    public static readonly string InfiniteBullnessPath = "vfx/vfx_infinite_bullness";
    /// <summary>
    /// 去掉前缀 <c>vfx/</c>，按下划线分段，每段首字母大写后首尾拼接（无分隔符）。
    /// 例如 <c>vfx/vfx_infinite_bullness</c> → <c>VfxInfiniteBullness</c>。
    /// </summary>
    private static string ConvertPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        string tail = path.Replace("vfx/", "", StringComparison.Ordinal);
        string[] segments = tail.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(segments.Select(static s => s.Capitalize()));
    }
    /// <summary>
    /// 添加指定目标的VFX
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public static void AddVfx(Creature target, string path){
        if(target == null) return;
        if (!TestMode.IsOn && NCombatRoom.Instance != null && !target.IsDead)
        {
            NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
            if (creatureNode != null)
            {
                string scenePath = SceneHelper.GetScenePath(path);
                Node2D node2D = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
                creatureNode.AddChildSafely(node2D);
                node2D.GlobalPosition = creatureNode.GlobalPosition;
                Log.Info($"添加VFX成功: {target.Name}, {path}");
            }
        }
    }
    /// <summary>
    /// 添加指定目标的VFX
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public static void AddVfxOnCenter(Creature target, string path){
        if(target == null) return;
        if (!TestMode.IsOn && NCombatRoom.Instance != null && !target.IsDead)
        {
            NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
            if (creatureNode != null)
            {
                string scenePath = SceneHelper.GetScenePath(path);
                Node2D node2D = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
                creatureNode.AddChildSafely(node2D);
                node2D.GlobalPosition = creatureNode.VfxSpawnPosition;
                Log.Info($"添加VFX成功: {target.Name}, {path}");
            }
        }
    }
    /// <summary>
    /// 删除指定目标的VFX
    /// </summary>
    public static void RemoveVfx<T>(Creature target) where T : Node2D{
        if(target == null) return;
        if (!TestMode.IsOn && NCombatRoom.Instance != null && !target.IsDead)
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
        Log.Info($"尝试移除VFX: {parent}, {typeof(T).Name}");
        if (!TestMode.IsOn && NCombatRoom.Instance != null)
        {
            // 获取VFX节点
            Log.Info($"获取VFX节点: {typeof(T).Name}");
            // 遍历所有子节点， 找到类型为T的节点，然后删除
            foreach(var child in parent.GetChildren()){
                if(child is T node){
                    node.QueueFreeSafely();
                    Log.Info($"移除VFX成功: {parent}, {typeof(T).Name}");
                    return;
                }
            }
        }
    }
}