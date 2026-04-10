//****************** 代码文件申明 ***********************
//* 文件：NHealthBarMorbidForegroundPatch
//* 作者：wheat
//* 创建时间：2026/04/10
//* 描述：在 NHealthBar._Ready 挂载病态层数前景条，并在 RefreshForeground 后与原版血条同步
//*******************************************************

using System.Reflection;
using System.Runtime.CompilerServices;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 病态层数在血条上的显示：控件挂在原版 <see cref="NHealthBar"/> 上，引用存于弱表，不写进游戏类型。
/// </summary>
[HarmonyPatch]
public static class NHealthBarMorbidForegroundPatch
{
	/// <summary>场景树里病态条的节点名，便于调试时在编辑器里辨认。</summary>
	private const string MorbidNodeName = "MorbidForeground_BiliBiliACGN";

	/// <summary>每个血条实例对应一条病态前景；弱引用避免阻挡 GC，血条释放后条目自动消失。</summary>
	private static readonly ConditionalWeakTable<NHealthBar, Control> MorbidForegroundByBar = new();

	/// <summary>原版私有方法 <c>GetFgWidth(int)</c>，用于把层数换算成与 HP 条一致的像素宽度。</summary>
	private static readonly Lazy<MethodInfo?> GetFgWidthInt = new(() =>
		AccessTools.DeclaredMethod(typeof(NHealthBar), "GetFgWidth", [typeof(int)]));

	/// <summary>原版私有属性 <c>MaxFgWidth</c>，前景条右边界与血条容器宽度对齐时用。</summary>
	private static readonly Lazy<PropertyInfo?> MaxFgWidthProp = new(() =>
		AccessTools.DeclaredProperty(typeof(NHealthBar), "MaxFgWidth"));

	/// <summary>读取 <c>NHealthBar</c> 上绑定的 <see cref="Creature"/>（私有字段 <c>_creature</c>）。</summary>
	private static Creature? GetCreature(NHealthBar bar) =>
		AccessTools.Field(typeof(NHealthBar), "_creature")?.GetValue(bar) as Creature;

	/// <summary>取末日条作克隆模板，布局与原版末日前景一致。</summary>
	private static Control? GetDoomForeground(NHealthBar bar) =>
		AccessTools.Field(typeof(NHealthBar), "_doomForeground")?.GetValue(bar) as Control;

	/// <summary>
	/// 在原版 <c>_Ready</c> 取完节点之后执行：复制末日条、改色为粉红、插入到末日条之后并登记弱表。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(typeof(NHealthBar), "_Ready")]
	public static void Ready_Postfix(NHealthBar __instance)
	{
		// 已创建且节点仍有效则跳过，防止重复 AddChild
		if (MorbidForegroundByBar.TryGetValue(__instance, out var existing) && IsValidControl(existing))
			return;

		var template = GetDoomForeground(__instance);
		if (template == null || template.GetParent() is not { } parent)
			return;

		var morbid = template.Duplicate() as Control;
		if (morbid == null)
			return;

		morbid.Name = MorbidNodeName;
		morbid.Visible = false;
		// 修改颜色（粉红色）并使用父材质
		morbid.SelfModulate = new Color(1f, 0.55f, 0.75f,0.5f);
		morbid.UseParentMaterial = true;

		parent.AddChild(morbid);
		// 画在末日条之上，叠放顺序与毒/末日类似时可再调 MoveChild 的索引`
		parent.MoveChild(morbid, template.GetIndex() + 1);
		MorbidForegroundByBar.Add(__instance, morbid);
	}

	/// <summary>
	/// 在原版 <c>RefreshForeground</c> 之后执行：按 <see cref="MorbidPower"/> 层数更新宽度与显隐；
	/// 算法对齐原版末日条非致命分支（<c>GetFgWidth</c> + <c>MaxFgWidth</c> + NinePatch 右边距）。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
	public static void RefreshForeground_Postfix(NHealthBar __instance)
	{
		if (!MorbidForegroundByBar.TryGetValue(__instance, out var morbid) || !IsValidControl(morbid))
			return;

		var creature = GetCreature(__instance);
		// 死亡、无限血与原版一致：不显示毒/末日，病态条也隐藏
		if (creature == null || creature.CurrentHp <= 0 || creature.ShowsInfiniteHp)
		{
			morbid.Visible = false;
			return;
		}

		// 预估下一回合造成的伤害
		var amount = creature.GetPower<MorbidPower>()?.CalculateTotalDamageNextTurn() ?? 0;
		if (amount <= 0)
		{
			morbid.Visible = false;
			return;
		}

		var getFg = GetFgWidthInt.Value;
		var maxProp = MaxFgWidthProp.Value;
		if (getFg == null || maxProp == null)
		{
			morbid.Visible = false;
			return;
		}

		morbid.Visible = true;
		var maxFg = (float)maxProp.GetValue(__instance)!;
		var fgForStacks = (float)getFg.Invoke(__instance, [amount])!;
		var offsetRight = fgForStacks - maxFg;

		morbid.OffsetLeft = 0f;
		if (morbid is NinePatchRect npr)
		{
			var patchMarginRight = npr.PatchMarginRight;
			morbid.OffsetRight = Mathf.Min(0f, offsetRight + patchMarginRight);
		}
		else
		{
			morbid.OffsetRight = offsetRight;
		}
	}

	/// <summary>节点未被释放（Godot 侧仍有效）。</summary>
	private static bool IsValidControl(Control? c) =>
		c != null && GodotObject.IsInstanceValid(c);
}
