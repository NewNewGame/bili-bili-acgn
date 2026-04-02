//****************** 代码文件申明 ***********************
//* 文件：NMapScreenRecalculateTravelabilityPatch
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：在 NMapScreen.RecalculateTravelability 原版前后插入逻辑（不替换原版）
//*******************************************************

using System.Collections;
using System.Linq;
using System.Reflection;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 在 <c>NMapScreen.RecalculateTravelability</c> 执行前（Prefix）与执行后（Postfix）插入模组逻辑；
/// Prefix 不返回 false，原版方法会照常运行。
/// </summary>
[HarmonyPatch]
public static class NMapScreenRecalculateTravelabilityPatch
{
    private const string LogTag = "[B站动画区][NMapPatch]";

    private static MethodBase? _target;

    /// <summary>
    /// 找不到目标方法时跳过本补丁，避免启动报错（游戏版本变更时需核对类名是否仍为 NMapScreen）。
    /// </summary>
    [HarmonyPrepare]
    public static bool Prepare()
    {
        _target = ResolveTarget();
        if (_target == null)
            Log.Error($"{LogTag} 未找到 NMapScreen.RecalculateTravelability，地图通行补丁未应用");
        return _target != null;
    }

    public static MethodBase TargetMethod() => _target!;
    /// <summary>
    /// 重新刷新地图
    /// </summary>
    public static void TryRefreshMap(){
        // 重新计算地图
        InvokeRecalculateTravelabilityReflect(NMapScreen.Instance);
        // 刷新地图
        RefreshMapVisual();
    }
    /// <summary>
    /// 反射调用 <c>NMapScreen</c> 实例上的 <c>private void RecalculateTravelability()</c>，走完整原版流程并触发本类的 Harmony Postfix。
    /// </summary>
    private static void InvokeRecalculateTravelabilityReflect(NMapScreen? instance)
    {
        if (instance is null)
            return;
        var mb = _target ?? AccessTools.Method(typeof(NMapScreen), "RecalculateTravelability", Type.EmptyTypes);
        if (mb == null)
            return;
        mb.Invoke(instance, null);
    }

    /// <summary>
    /// 在原版重算结束后，按 <see cref="UseFlightStyleNextRow"/> 再应用一层与原版 Flight 分支等价的可通行扩展。
    /// </summary>
    [HarmonyPostfix]
    public static void Postfix(object __instance)
    {
        if (__instance is null)
            return;
        try
        {   
            var runState = RunManager.Instance.DebugOnlyGetState();
            if (runState == null)
                return;
            ApplyTravelabilityBranchLikeOriginal(__instance, IsPlayerUsingFly(runState));
        }
        catch (Exception ex)
        {
            Log.Error($"{LogTag} ApplyTravelabilityBranchLikeOriginal 异常: {ex}");
        }
    }
    /// <summary>
    /// 只要玩家持有伊蕾娜的扫帚，就开启飞行
    /// </summary>
    /// <param name="runState"></param>
    /// <returns></returns>
    private static bool IsPlayerUsingFly(RunState runState)
    {
        // 如果开启飞行了，则直接返回true
        if(runState.Modifiers.OfType<Flight>().Any())
            return true;
        // 否则遍历玩家，只要玩家持有伊蕾娜的扫帚，就返回true
        foreach (var player in runState.Players)
        {
            if (player.Relics.OfType<ElainasBroom>().Any())
                return true;
        }
        return false;
    }
    /// <summary>
    /// 刷新地图
    /// </summary>
    public static void RefreshMapVisual()
    {
        NMapScreen.Instance?.RefreshAllPointVisuals();
    }
    /// <summary>
    /// 对应原版片段：
    /// if (mapCoord.row != _map.GetRowCount() - 1) {
    ///   var enumerable = flight ? _map.GetPointsInRow(mapCoord.row + 1) : _mapPointDictionary[mapCoord].Point.Children;
    ///   foreach (MapPoint item in enumerable) { _mapPointDictionary[item.coord].State = MapPointState.Travelable; }
    /// }
    /// </summary>
    private static void ApplyTravelabilityBranchLikeOriginal(object nMapScreen, bool useFly)
    {
        if (useFly == false)
            return;
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
            return;
        if (runState.Modifiers.OfType<Flight>().Any())
            return;
        if (!runState.VisitedMapCoords.Any())
            return;

        // 获取最后一个访问的坐标
        var mapCoord = runState.VisitedMapCoords.Last();
        if (mapCoord == null)
            return;
        var screenType = nMapScreen.GetType();
        var map = AccessTools.DeclaredField(screenType, "_map")?.GetValue(nMapScreen);
        var dictObj = AccessTools.DeclaredField(screenType, "_mapPointDictionary")?.GetValue(nMapScreen);
        if (map == null || dictObj == null)
            return;

        var mapType = map.GetType();
        var getRowCount = AccessTools.Method(mapType, "GetRowCount");
        var getPointsInRow = AccessTools.Method(mapType, "GetPointsInRow", [typeof(int)]);
        var rowCountObj = getRowCount?.Invoke(map, null);
        var rowCount = rowCountObj is int i ? i : rowCountObj is long l ? (int)l : (int?)null;
        if (rowCount is null || getPointsInRow == null)
            return;

        if (dictObj is not IDictionary rawDict)
            return;

        // 遍历字典中的楼层
        foreach (var keyObj in rawDict.Keys)
        {
            if (keyObj is null)
                continue;

            var row = GetMapCoordRow(keyObj);
            if (row is null)
                continue;

            // 如果当前行是最后一行，或者当前行不是下一个要访问的行，则跳过
            if (row.Value == rowCount.Value - 1 || row.Value != mapCoord.row)
                continue;

            IEnumerable? enumerable = getPointsInRow.Invoke(map, [row.Value + 1]) as IEnumerable;
            if (enumerable == null)
                continue;

            foreach (var item in enumerable)
            {
                if (item is null)
                    continue;

                var coord = GetMapPointCoordOrConstruct(item, rawDict);
                if (coord == null)
                    continue;

                if (!rawDict.Contains(coord))
                    continue;

                var entry = rawDict[coord];
                if (entry is null)
                    continue;

                SetMapPointNodeStateTravelable(entry);
            }
        }
    }

    private const BindingFlags InstanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// 从 <see cref="MapPoint"/> 取与 <c>_mapPointDictionary</c> 键一致的坐标。
    /// STS2 的 <c>MapPoint</c> 可能无 public <c>coord</c>，或坐标在 <c>MapCoord</c> / 字段里；必要时用 Row+Column + 字典键类型上的 (int,int) 构造。
    /// </summary>
    private static object? GetMapPointCoordOrConstruct(object mapPoint, IDictionary rawDict)
    {
        var direct = GetMapPointCoord(mapPoint);
        if (direct != null)
            return direct;

        object? sampleKey = null;
        foreach (var k in rawDict.Keys)
        {
            if (k != null)
            {
                sampleKey = k;
                break;
            }
        }
        if (sampleKey == null)
            return null;

        var keyType = sampleKey.GetType();
        var t = mapPoint.GetType();
        if (keyType.IsInstanceOfType(mapPoint))
            return mapPoint;

        foreach (var p in t.GetProperties(InstanceAny))
        {
            if (p.GetIndexParameters().Length > 0)
                continue;
            var v = p.GetValue(mapPoint);
            if (v == null)
                continue;
            if (keyType.IsAssignableFrom(v.GetType()) || v.GetType() == keyType)
                return v;
            if (p.Name.Contains("Coord", StringComparison.OrdinalIgnoreCase) && keyType.IsInstanceOfType(v))
                return v;
        }

        foreach (var f in t.GetFields(InstanceAny))
        {
            var v = f.GetValue(mapPoint);
            if (v != null && keyType.IsInstanceOfType(v))
                return v;
        }

        var row = TryGetIntComponent(mapPoint, t, "row", "Row", "Y");
        var col = TryGetIntComponent(mapPoint, t, "column", "Column", "Col", "col", "X");
        if (row is null || col is null)
            return null;

        foreach (var ctor in keyType.GetConstructors(InstanceAny))
        {
            var ps = ctor.GetParameters();
            if (ps.Length != 2 || ps[0].ParameterType != typeof(int) || ps[1].ParameterType != typeof(int))
                continue;
            try
            {
                return ctor.Invoke([row.Value, col.Value]);
            }
            catch
            {
                try
                {
                    return ctor.Invoke([col.Value, row.Value]);
                }
                catch
                {
                    // 下一构造
                }
            }
        }

        return null;
    }

    private static object? GetMapPointCoord(object mapPoint)
    {
        var t = mapPoint.GetType();
        foreach (var name in new[] { "coord", "Coord", "MapCoord", "mapCoord", "Cell", "Key" })
        {
            var p = t.GetProperty(name, InstanceAny);
            if (p?.GetIndexParameters().Length == 0)
            {
                var v = p.GetValue(mapPoint);
                if (v != null)
                    return v;
            }
        }
        foreach (var name in new[] { "coord", "_coord", "MapCoord", "_mapCoord" })
        {
            var f = t.GetField(name, InstanceAny);
            if (f != null)
            {
                var v = f.GetValue(mapPoint);
                if (v != null)
                    return v;
            }
        }
        return null;
    }

    private static int? TryGetIntComponent(object target, Type t, params string[] names)
    {
        foreach (var name in names)
        {
            var p = t.GetProperty(name, InstanceAny);
            if (p?.GetIndexParameters().Length == 0)
            {
                var v = p.GetValue(target);
                if (v is int i)
                    return i;
                if (v is long l)
                    return (int)l;
            }
            var f = t.GetField(name, InstanceAny);
            if (f != null)
            {
                var v = f.GetValue(target);
                if (v is int i2)
                    return i2;
                if (v is long l2)
                    return (int)l2;
            }
        }
        return null;
    }

    private static int? GetMapCoordRow(object mapCoord)
    {
        var t = mapCoord.GetType();
        if (t.GetField("row", InstancePublic)?.GetValue(mapCoord) is int r)
            return r;
        if (t.GetProperty("row", InstancePublic)?.GetValue(mapCoord) is int r2)
            return r2;
        if (t.GetProperty("Row", InstancePublic)?.GetValue(mapCoord) is int r3)
            return r3;
        return null;
    }

    private static void SetMapPointNodeStateTravelable(object mapPointDictionaryValue)
    {
        var stateProp = mapPointDictionaryValue.GetType().GetProperty("State", InstancePublic);
        if (stateProp?.PropertyType is not { IsEnum: true } stateType)
            return;
        var travelable = Enum.GetValues(stateType).Cast<object>()
            .FirstOrDefault(v => v.ToString() == "Travelable");
        if (travelable != null)
            stateProp.SetValue(mapPointDictionaryValue, travelable);
    }

    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance;

    private static MethodBase? ResolveTarget()
    {
        try
        {
            foreach (var t in typeof(ModelDb).Assembly.GetTypes())
            {
                if (t.Name != "NMapScreen")
                    continue;
                var m = AccessTools.Method(t, "RecalculateTravelability", Type.EmptyTypes);
                if (m != null)
                    return m;
            }
        }
        catch (ReflectionTypeLoadException)
        {
        }

        return null;
    }
}
