//****************** 代码文件申明 ***********************
//* 文件：SNBerserkVfx(红怒VFX)
//* 作者：wheat
//* 创建时间：2026/04/12
//* 描述：红怒VFX
//*******************************************************

using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

public partial class SNBerserkVfx : Node2D
{
    public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_berserk");

	[Export(PropertyHint.None, "")]
	private Array<GpuParticles2D> _particles = new Array<GpuParticles2D>();

	private Color _tint = Colors.White;

	private CancellationTokenSource? _cts;

	public static SNBerserkVfx? Create(Vector2 targetPosition, Color tint)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		SNBerserkVfx nBerserkVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<SNBerserkVfx>(PackedScene.GenEditState.Disabled);
		nBerserkVfx.GlobalPosition = targetPosition;
		nBerserkVfx._tint = tint;
		return nBerserkVfx;
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
		PlayVfx();
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private void PlayVfx()
	{
		_cts = new CancellationTokenSource();
		foreach (GpuParticles2D particle in _particles)
		{
			particle.SelfModulate = _tint;
			particle.Restart();
		}
	}
}