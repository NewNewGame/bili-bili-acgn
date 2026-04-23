//****************** 代码文件申明 ***********************
//* 文件：SNSfxPooledPlayer
//* 作者：wheat
//* 创建时间：2026/04/23 00:00:00 星期四
//* 描述：SFX 播放节点（对象池复用），播放结束归还
//*******************************************************

using Godot;
using System;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

public partial class SNSfxPooledPlayer : Node
{
    private AudioStreamPlayer? _player;
    private Action<SNSfxPooledPlayer>? _returnToPool;

    public override void _Ready()
    {
        _player ??= GetNodeOrNull<AudioStreamPlayer>("Player");
        if (_player == null)
            return;

        // 防止重复连接
        if (!_player.IsConnected(AudioStreamPlayer.SignalName.Finished, Callable.From(OnFinished)))
        {
            _player.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(OnFinished));
        }
    }

    public void Play(
        AudioStream stream,
        Action<SNSfxPooledPlayer> returnToPool,
        float volumeDb = 0f,
        float pitchScale = 1f,
        string? bus = null
    )
    {
        _player ??= GetNodeOrNull<AudioStreamPlayer>("Player");
        if (_player == null)
            return;

        _returnToPool = returnToPool;
        _player.Stream = stream;
        _player.VolumeDb = volumeDb;
        _player.PitchScale = pitchScale;
        if (!string.IsNullOrWhiteSpace(bus))
            _player.Bus = bus!;

        _player.Play();
    }

    private void OnFinished()
    {
        if (_player != null)
        {
            _player.Stop();
            _player.Stream = null;
        }

        _returnToPool?.Invoke(this);
    }
}

