//****************** 代码文件申明 ***********************
//* 文件：SNTheWorldVfx
//* 作者：wheat
//* 创建时间：2026/05/05 12:00:00 星期二
//* 描述：The World 风格时停领域 VFX：球形扩张、负片、向心扭曲、白色收缩环
//*******************************************************

using System;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

public partial class SNTheWorldVfx : Node2D
{
	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_the_world");

	private const string FieldNodeName = "TheWorldField";

	[Export]
	public TheWorldVfxConfig? Config { get; set; }

	[Export(PropertyHint.None, "")]
	private Array<GpuParticles2D> _ringParticleLayers = new Array<GpuParticles2D>();

	private Polygon2D? _field;
	private ShaderMaterial? _fieldMaterial;
	private TheWorldVfxConfig _cfg = null!;
	private float _elapsed;
	private CancellationTokenSource? _cts;

	public static SNTheWorldVfx? Create(Vector2 targetPosition, TheWorldVfxConfig? config = null)
	{
		if (TestMode.IsOn)
		{
			return null;
		}

		SNTheWorldVfx vfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<SNTheWorldVfx>(PackedScene.GenEditState.Disabled);
		vfx.GlobalPosition = targetPosition;
		vfx.Config = config;
		return vfx;
	}

	public override void _Ready()
	{
		_cfg = Config ?? TheWorldVfxConfig.CreateDefault();
		_field = GetNodeOrNull<Polygon2D>(FieldNodeName);
		if (_field?.Material is ShaderMaterial fieldMat)
		{
			_fieldMaterial = (ShaderMaterial)fieldMat.Duplicate(true);
			_field.Material = _fieldMaterial;
		}
		if (_ringParticleLayers.Count == 0)
		{
			foreach (Node child in GetChildren())
			{
				if (child is GpuParticles2D gpu && child.Name.ToString().StartsWith("RingWave", StringComparison.Ordinal))
				{
					_ringParticleLayers.Add(gpu);
				}
			}
		}

		PrepareRingLayers();
		TaskHelper.RunSafely(PlayVfx());
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	public override void _Process(double delta)
	{
		if (_fieldMaterial == null)
		{
			return;
		}

		_elapsed += (float)delta;
		UpdateFieldShader();
	}

	private void PrepareRingLayers()
	{
		foreach (GpuParticles2D ring in _ringParticleLayers)
		{
			if (ring.ProcessMaterial is ParticleProcessMaterial pm)
			{
				ring.ProcessMaterial = (ParticleProcessMaterial)pm.Duplicate(true);
			}

			ring.Lifetime = _cfg.RingParticleLifetime;
			ring.Emitting = false;
		}
	}

	private void UpdateFieldShader()
	{
		ShaderMaterial? mat = _fieldMaterial;
		if (mat == null)
		{
			return;
		}

		Viewport vp = GetViewport();
		Vector2 vsize = vp.GetVisibleRect().Size;
		Transform2D xf = vp.GetCanvasTransform() * GetGlobalTransformWithCanvas();
		Vector2 centerPx = xf * Vector2.Zero;

		float tExpand = Mathf.Clamp(_elapsed / Mathf.Max(0.0001f, _cfg.SphereExpandDuration), 0f, 1f);
		float eased = EaseOutCubic(tExpand);
		float radius = Mathf.Lerp(_cfg.MinSphereRadiusPx, _cfg.MaxSphereRadiusPx, eased);

		float distort01 = 1f;
		if (_elapsed >= _cfg.DistortFadeStart)
		{
			float span = Mathf.Max(0.0001f, _cfg.DistortFadeEnd - _cfg.DistortFadeStart);
			distort01 = 1f - Mathf.Clamp((_elapsed - _cfg.DistortFadeStart) / span, 0f, 1f);
		}

		float invertFadeStart = _cfg.DistortFadeEnd + _cfg.InvertOnlyHoldDuration;
		float invertFadeEnd = invertFadeStart + _cfg.InvertFadeOutDuration;
		float invert01 = 1f;
		if (_elapsed >= invertFadeStart)
		{
			invert01 = 1f - Mathf.Clamp((_elapsed - invertFadeStart) / Mathf.Max(0.0001f, invertFadeEnd - invertFadeStart), 0f, 1f);
		}

		mat.SetShaderParameter("u_viewport_size", vsize);
		mat.SetShaderParameter("u_center_px", centerPx);
		mat.SetShaderParameter("u_sphere_radius_px", radius);
		mat.SetShaderParameter("u_edge_soft_px", _cfg.EdgeSoftnessPx);
		mat.SetShaderParameter("u_distort_strength", _cfg.MaxDistortStrength * distort01);
		mat.SetShaderParameter("u_invert_strength", invert01);
		mat.SetShaderParameter("u_gold_tint", _cfg.GoldTint);
	}

	private async Task PlayVfx()
	{
		_cts = new CancellationTokenSource();
		CancellationToken token = _cts.Token;

		await Cmd.Wait(_cfg.RingPhaseStart, token);
		float ringOuter = _cfg.MaxSphereRadiusPx * _cfg.RingEmissionRadiusScale;
		float ringInner = Mathf.Max(ringOuter - 80f, _cfg.MinSphereRadiusPx + 20f);
		for (int i = 0; i < _ringParticleLayers.Count; i++)
		{
			GpuParticles2D ring = _ringParticleLayers[i];
			if (ring.ProcessMaterial is ParticleProcessMaterial pm)
			{
				pm.EmissionRingRadius = ringOuter;
				pm.EmissionRingInnerRadius = ringInner;
			}

			ring.Restart();
			if (i < _ringParticleLayers.Count - 1)
			{
				await Cmd.Wait(_cfg.RingWaveInterval, token);
			}
		}

		float fullDuration = _cfg.DistortFadeEnd + _cfg.InvertOnlyHoldDuration + _cfg.InvertFadeOutDuration + _cfg.CleanupTail;
		int ringCount = _ringParticleLayers.Count;
		float ringSequenceEnd = _cfg.RingPhaseStart + _cfg.RingWaveInterval * Mathf.Max(0, ringCount - 1);
		float remain = Mathf.Max(0f, fullDuration - ringSequenceEnd);
		await Cmd.Wait(remain, token);
		this.QueueFreeSafely();
	}

	private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
}
