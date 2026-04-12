//****************** 代码文件申明 ***********************
//* 文件：SNInfiniteBullnessVfx(无限牛处VFX)
//* 作者：wheat
//* 创建时间：2026/04/12
//* 描述：无限牛处VFX
//*******************************************************

using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

public partial class SNInfiniteBullnessVfx : Node2D
{
    public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_infinite_bullness");

	[Export(PropertyHint.None, "")]
	private Array<GpuParticles2D> _particles = new Array<GpuParticles2D>();

	private Color _tint = Colors.White;

	private CancellationTokenSource? _cts;

	public static SNInfiniteBullnessVfx? Create(Vector2 targetPosition, Color tint)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		SNInfiniteBullnessVfx nSplashVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<SNInfiniteBullnessVfx>(PackedScene.GenEditState.Disabled);
		nSplashVfx.GlobalPosition = targetPosition;
		nSplashVfx._tint = tint;
		return nSplashVfx;
	}

	public override void _Ready()
	{
		if (_particles.Count == 0)
		{
			foreach (Node child in GetChildren())
			{
				if (child is GpuParticles2D gpu)
				{
					_particles.Add(gpu);
				}
			}
		}
		TaskHelper.RunSafely(PlayVfx());
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async Task PlayVfx()
	{
		_cts = new CancellationTokenSource();
		foreach (GpuParticles2D particle in _particles)
		{
			particle.SelfModulate = _tint;
			particle.Restart();
		}
		await Cmd.Wait(1.35f, _cts.Token);
		this.QueueFreeSafely();
	}
}