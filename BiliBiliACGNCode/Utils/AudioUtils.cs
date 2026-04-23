//****************** 代码文件申明 ***********************
//* 文件：AudioUtils
//* 作者：wheat
//* 创建时间：2026/04/23
//* 描述：音频工具类
//*******************************************************

using Godot;
using System.Collections.Generic;
using BiliBiliACGN.BiliBiliACGNCode.Nodes;
using System;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class AudioUtils
{
    #region 资源配置
    private static readonly object _defaultParentLock = new();
    private static Node? _defaultAudioManagerParent;
    private static Node? _defaultPoolParent;
    private static readonly object _sfxCacheLock = new();
    private static readonly Dictionary<string, AudioStream> _sfxCache = new();

    private static readonly object _sfxPoolLock = new();
    private static readonly Stack<SNSfxPooledPlayer> _sfxPlayerPool = new();
    private static PackedScene? _sfxPlayerPrefab;

    private const string SfxPlayerPrefabPath = "res://BiliBiliACGN/scenes/sfxPrefab/SfxPlayer.tscn";
    private static readonly StringName BottleAttackEventPath = new StringName("event:/sfx/characters/bottle/bottle_attack");
    public static readonly StringName BersekEnterEventPath = new StringName("res://BiliBiliACGN/sfx/powers/berserk_enter.ogg");
    #endregion

    #region 初始化
    private static string[] _sfxPaths = new string[]
    {
        BersekEnterEventPath,
    };
    /// <summary>
    /// 音效路径集合
    /// 初始化的时候加载这些音效到缓存
    /// </summary>
    private static string[] _bottleAttackSfxPaths = new string[]
    {
        "res://BiliBiliACGN/sfx/attack/Sweep_attack0.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack1.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack2.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack3.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack4.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack5.ogg",
        "res://BiliBiliACGN/sfx/attack/Sweep_attack6.ogg",
    };
    /// <summary>
    /// 音效事件路径集合
    /// 不在里面的不处理
    /// </summary>
    private static HashSet<StringName> pathSets = new HashSet<StringName>(){
        BottleAttackEventPath,
        BersekEnterEventPath,
    };
    private static bool _initialized = false;
    /// <summary>
    /// 初始化加载音效缓存
    /// </summary>
    public static void Initialize()
    {
        if (_initialized)
            return;
        _initialized = true;

        // 加载音效缓存
        ClearSfxCache();
        ClearSfxNodePool();
        GetSfxPlayerPrefab();
        // 加载音效缓存
        foreach (var path in _bottleAttackSfxPaths)
        {
            LoadSfxCached(path);
        }
        foreach (var path in _sfxPaths)
        {
            LoadSfxCached(path);
        }
    }
    #endregion
    
    #region 默认节点管理
    /// <summary>
    /// 设置默认的音效父节点（通常是全局 AudioManager）。
    /// </summary>
    public static void SetDefaultAudioManagerParent(Node? parent)
    {
        if (parent != null && !GodotObject.IsInstanceValid(parent))
            parent = null;

        lock (_defaultParentLock)
        {
            _defaultAudioManagerParent = parent;
        }
    }

    /// <summary>
    /// 获取默认的音效父节点；如果没有，会尝试在场景树中查找名为 "AudioManager" 的节点，
    /// 仍没有则创建一个名为 "MyAudioManager" 的新节点挂到 SceneTree.Root 下。
    /// </summary>
    public static Node? EnsureDefaultAudioManagerParent()
    {
        lock (_defaultParentLock)
        {
            if (_defaultAudioManagerParent != null && GodotObject.IsInstanceValid(_defaultAudioManagerParent))
                return _defaultAudioManagerParent;
        }

        var tree = Engine.GetMainLoop() as SceneTree;
        var root = tree?.Root;
        if (root == null || !GodotObject.IsInstanceValid(root))
            return null;

        // 1) 查找全局名为 AudioManager 的节点
        var found = root.FindChild("AudioManager", true, false) as Node;
        if (found != null && GodotObject.IsInstanceValid(found))
        {
            SetDefaultAudioManagerParent(found);
            return found;
        }

        // 2) 没找到就创建一个 MyAudioManager
        var created = new Node { Name = "MyAudioManager" };
        root.AddChild(created);
        SetDefaultAudioManagerParent(created);
        return created;
    }

    /// <summary>
    /// 尝试获取默认音效父节点（不创建）。如需要自动创建请用 <see cref="EnsureDefaultAudioManagerParent"/>.
    /// </summary>
    public static Node? GetDefaultAudioManagerParent()
    {
        lock (_defaultParentLock)
        {
            if (_defaultAudioManagerParent != null && GodotObject.IsInstanceValid(_defaultAudioManagerParent))
                return _defaultAudioManagerParent;
            return null;
        }
    }
    #endregion

    #region 资源管理
    /// <summary>
    /// 清空已缓存的音效资源引用（仅清理缓存引用；已在播放中的实例不受影响）。
    /// </summary>
    public static void ClearSfxCache()
    {
        lock (_sfxCacheLock)
        {
            _sfxCache.Clear();
        }
    }

    /// <summary>
    /// 清空对象池中的音效节点（会 QueueFree 这些空闲节点）。
    /// </summary>
    public static void ClearSfxNodePool()
    {
        lock (_sfxPoolLock)
        {
            while (_sfxPlayerPool.Count > 0)
            {
                var n = _sfxPlayerPool.Pop();
                if (GodotObject.IsInstanceValid(n))
                    n.QueueFree();
            }
        }
    }

    private static PackedScene? GetSfxPlayerPrefab()
    {
        if (_sfxPlayerPrefab != null)
            return _sfxPlayerPrefab;

        if (!ResourceLoader.Exists(SfxPlayerPrefabPath))
            return null;

        _sfxPlayerPrefab = ResourceLoader.Load<PackedScene>(SfxPlayerPrefabPath);
        return _sfxPlayerPrefab;
    }

    private static AudioStream? LoadSfxCached(string resourcePath)
    {
        lock (_sfxCacheLock)
        {
            if (_sfxCache.TryGetValue(resourcePath, out var cached) && cached != null)
                return cached;
        }

        if (!ResourceLoader.Exists(resourcePath))
            return null;

        var stream = ResourceLoader.Load<AudioStream>(resourcePath);
        if (stream == null)
            return null;

        lock (_sfxCacheLock)
        {
            _sfxCache[resourcePath] = stream;
        }

        LogUtils.LogInfo($"Sfx已缓存: {resourcePath}");

        return stream;
    }

    private static SNSfxPooledPlayer? RentSfxPlayer(Node poolParent)
    {
        lock (_sfxPoolLock)
        {
            while (_sfxPlayerPool.Count > 0)
            {
                var n = _sfxPlayerPool.Pop();
                if (GodotObject.IsInstanceValid(n))
                    return n;
            }
        }

        var prefab = GetSfxPlayerPrefab();
        if (prefab == null)
            return null;

        var inst = prefab.Instantiate<SNSfxPooledPlayer>(PackedScene.GenEditState.Disabled);
        return inst;
    }

    private static void ReturnSfxPlayer(SNSfxPooledPlayer player, Node poolParent)
    {
        if (!GodotObject.IsInstanceValid(player))
            return;

        if (GodotObject.IsInstanceValid(poolParent))
        {
            var curParent = player.GetParent();
            if (curParent != poolParent)
            {
                curParent?.RemoveChild(player);
                poolParent.AddChild(player);
            }
        }

        lock (_sfxPoolLock)
        {
            _sfxPlayerPool.Push(player);
        }
    }
    #endregion

    #region 播放音效
    private static SNSfxPooledPlayer? PlayOneShotSfxPooledInternal(
        string resourcePath,
        Node audioManagerParent,
        float volumeDb = 0f,
        float pitchScale = 1f,
        string? bus = null,
        Node? poolParent = null
    )
    {
        LogUtils.LogInfo($"播放音效开始: {resourcePath}");
        // 路径资源转换
        if(resourcePath == BottleAttackEventPath){
            resourcePath = GetRandomBottleAttackSfxPath();
        }
        if (string.IsNullOrWhiteSpace(resourcePath))
            return null;
        if (!GodotObject.IsInstanceValid(audioManagerParent))
            return null;
        var stream = LoadSfxCached(resourcePath);
        if (stream == null)
            return null;
        var actualPoolParent = EnsurePoolParent(audioManagerParent, poolParent);
        if (actualPoolParent == null)
            return null;
        var node = RentSfxPlayer(actualPoolParent);
        if (node == null)
            return null;
        // 播放时挂在 AudioManager 下；空闲时可回收到 poolParent（默认同一个）
        if (node.GetParent() != audioManagerParent)
        {
            node.GetParent()?.RemoveChild(node);
            audioManagerParent.AddChild(node);
        }
        node.Play(
            stream,
            p => ReturnSfxPlayer(p, actualPoolParent),
            volumeDb,
            pitchScale,
            bus
        );

        return node;
    }

    /// <summary>
    /// 确保池父节点
    /// 默认选择audioManagerParent下面的PoolParent节点，如果找不到则创建一个
    /// </summary>
    private static Node? EnsurePoolParent(Node audioManagerParent, Node? poolParent)
    {
        if(_defaultPoolParent != null && GodotObject.IsInstanceValid(_defaultPoolParent))
            return _defaultPoolParent;

        if (!GodotObject.IsInstanceValid(audioManagerParent))
            return null;

        if (poolParent != null && GodotObject.IsInstanceValid(poolParent))
            return poolParent;

        var found = audioManagerParent.GetNodeOrNull<Node>("PoolParent");
        if (found != null && GodotObject.IsInstanceValid(found))
            return _defaultPoolParent = found;

        var created = new Node { Name = "PoolParent" };
        audioManagerParent.AddChild(created);
        return _defaultPoolParent = created;
    }

    /// <summary>
    /// 对象池版本（使用默认 AudioManager 父节点）。
    /// </summary>
    public static SNSfxPooledPlayer? PlayOneShotSfx(
        string resourcePath,
        float volumeDb = 0f,
        float pitchScale = 1f,
        string? bus = null,
        Node? poolParent = null
    )
    {
        LogUtils.LogInfo($"尝试播放音效: {resourcePath}, 音量: {volumeDb}");
        // 不在集合里的不处理
        if(!pathSets.Contains(resourcePath))
            return null;
        var parent = EnsureDefaultAudioManagerParent();
        if (parent == null)
            return null;
        return PlayOneShotSfxPooledInternal(resourcePath, parent, volumeDb, pitchScale, bus, poolParent);
    }
    #endregion

    #region 音效事件
    /// <summary>
    /// 获取随机瓶子攻击音效路径
    /// </summary>
    /// <returns></returns>
    private static string GetRandomBottleAttackSfxPath()
    {
        return _bottleAttackSfxPaths[Random.Shared.Next(0, _bottleAttackSfxPaths.Length - 1)];
    }
    #endregion
}