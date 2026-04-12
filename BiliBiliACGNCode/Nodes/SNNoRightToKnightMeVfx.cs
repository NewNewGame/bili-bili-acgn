//****************** 代码文件申明 ***********************
//* 文件：SNNoRightToKnightMeVfx(无权为我授勋VFX)
//* 作者：wheat
//* 创建时间：2026/04/12
//* 描述：顶光、纸屑下落、星光闪烁，结束后回收节点
//*******************************************************

using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

public partial class SNNoRightToKnightMeVfx : Node2D
{
	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_no_right_to_knight_me");

	private const float VfxDurationSeconds = 2.65f;

	[Export(PropertyHint.None, "")]
	private Array<GpuParticles2D> _particles = new Array<GpuParticles2D>();

	private Color _tint = Colors.White;

	private CancellationTokenSource? _cts;

	public static SNNoRightToKnightMeVfx? Create(Vector2 targetPosition, Color tint)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		SNNoRightToKnightMeVfx vfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<SNNoRightToKnightMeVfx>(PackedScene.GenEditState.Disabled);
		vfx.GlobalPosition = targetPosition;
		vfx._tint = tint;
		return vfx;
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
		await Cmd.Wait(VfxDurationSeconds, _cts.Token);
		this.QueueFreeSafely();
	}
}
