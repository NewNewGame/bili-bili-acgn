//****************** 代码文件申明 ***********************
//* 文件：TheWorldVfxConfig
//* 作者：wheat
//* 创建时间：2026/05/05 12:00:00 星期二
//* 描述：The World 风格时停领域 VFX 的时间轴与强度配置（Resource）
//*******************************************************

using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Nodes;

// 可在检查器中新建 Resource 并挂到 SNTheWorldVfx 的 Config 导出字段，或使用代码默认值。
[GlobalClass]
public partial class TheWorldVfxConfig : Resource
{
	[Export(PropertyHint.Range, "0.05,4,0.01")]
	public float SphereExpandDuration { get; set; } = 1.0f;

	[Export(PropertyHint.Range, "200,6000,1")]
	public float MaxSphereRadiusPx { get; set; } = 2600.0f;

	[Export(PropertyHint.Range, "8,200,1")]
	public float MinSphereRadiusPx { get; set; } = 48.0f;

	[Export(PropertyHint.Range, "20,400,1")]
	public float EdgeSoftnessPx { get; set; } = 150.0f;

	[Export(PropertyHint.Range, "0,1,0.01")]
	public float MaxDistortStrength { get; set; } = 0.48f;

	/// <summary>从开始后到扭曲开始衰减的时间（秒）。</summary>
	[Export(PropertyHint.Range, "0,6,0.05")]
	public float DistortFadeStart { get; set; } = 1.15f;

	/// <summary>扭曲完全消失的时间（秒）。</summary>
	[Export(PropertyHint.Range, "0,8,0.05")]
	public float DistortFadeEnd { get; set; } = 2.05f;

	[Export(PropertyHint.Range, "0,0.4,0.01")]
	public float GoldTint { get; set; } = 0.12f;

	[Export(PropertyHint.Range, "0,3,0.05")]
	public float RingPhaseStart { get; set; } = 0.78f;

	[Export(PropertyHint.Range, "0.02,0.5,0.01")]
	public float RingWaveInterval { get; set; } = 0.14f;

	[Export(PropertyHint.Range, "0.4,2.5,0.05")]
	public float RingParticleLifetime { get; set; } = 0.82f;

	[Export(PropertyHint.Range, "200,3200,10")]
	public float RingEmissionRadiusScale { get; set; } = 0.92f;

	/// <summary>扭曲结束后仅保留负片色的时长（秒）。</summary>
	[Export(PropertyHint.Range, "0,10,0.05")]
	public float InvertOnlyHoldDuration { get; set; } = 1.6f;

	[Export(PropertyHint.Range, "0.1,3,0.05")]
	public float InvertFadeOutDuration { get; set; } = 0.55f;

	[Export(PropertyHint.Range, "0,2,0.05")]
	public float CleanupTail { get; set; } = 0.2f;

	public static TheWorldVfxConfig CreateDefault() => new TheWorldVfxConfig();
}
