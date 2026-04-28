//****************** 代码文件申明 ***********************
//* 文件：NOrbUpdateVisualsPatch
//* 作者：wheat
//* 创建时间：2026/04/14 10:00:00 星期二
//* 描述：在 NOrb.UpdateVisuals 原版执行后插入自定义球体表现逻辑
//*******************************************************

using System.Reflection;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Orbs;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(NOrb))]
public static class NOrbUpdateVisualsPatch
{
	private static readonly Lazy<FieldInfo?> PassiveLabelField = new(() =>
		AccessTools.DeclaredField(typeof(NOrb), "_passiveLabel"));

	private static readonly Lazy<FieldInfo?> EvokeLabelField = new(() =>
		AccessTools.DeclaredField(typeof(NOrb), "_evokeLabel"));

	/// <summary>首次使用时解析 <c>_passiveLabel</c> 字段并复用，避免每次 <c>GetValue</c> 前重复取 <see cref="FieldInfo"/>。</summary>
	private static readonly Lazy<Func<NOrb, object?>> ReadPassiveLabel = new(() =>
	{
		FieldInfo? f = PassiveLabelField.Value;
		return orb => f?.GetValue(orb);
	});

	/// <summary>同上，对应 <c>_evokeLabel</c>。</summary>
	private static readonly Lazy<Func<NOrb, object?>> ReadEvokeLabel = new(() =>
	{
		FieldInfo? f = EvokeLabelField.Value;
		return orb => f?.GetValue(orb);
	});

	[HarmonyPostfix]
	[HarmonyPatch(nameof(NOrb.UpdateVisuals))]
	public static void UpdateVisuals_Postfix(NOrb __instance, bool isEvoking)
	{
		if (!__instance.IsNodeReady() || !CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (__instance.Model == null)
		{
			return;
		}
		OrbModel model = __instance.Model;
		if (model is not StrengthOrb)
		{
			return;
		}
		// 获取被动和激发标签
		var passive = ReadPassiveLabel.Value(__instance);
		var evoke = ReadEvokeLabel.Value(__instance);
		// 如果被动和激发标签存在，则设置标签的可见性和文本
		if (passive is MegaLabel passiveLabel && evoke is MegaLabel evokeLabel)
		{
			passiveLabel.Visible = true;
			evokeLabel.Visible = true;
			passiveLabel.SetTextAutoSize(model.PassiveVal.ToString("0") + "/3");
			evokeLabel.SetTextAutoSize((model.EvokeVal / 3m).ToString("0"));
		}
	}
}
