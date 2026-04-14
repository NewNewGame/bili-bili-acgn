//****************** 代码文件申明 ***********************
//* 文件：NHealthBarPatch
//* 作者：wheat
//* 创建时间：2026/04/10
//* 描述：更新拓展血条的显示，显示病态条
//*******************************************************

using System.Reflection;
using System.Runtime.CompilerServices;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;


[HarmonyPatch(typeof(NHealthBar))]
public static class NHealthBarPatch
{

	#region 血条相关字段属性
	/// <summary>血条的文本标签。</summary>
	private static readonly Lazy<FieldInfo?> HpLabelProp = new(() =>
		AccessTools.DeclaredField(typeof(NHealthBar), "_hpLabel"));
	/// <summary>血条的文本标签。</summary>
	private static readonly Lazy<FieldInfo?> CreatureProp = new(() =>
		AccessTools.DeclaredField(typeof(NHealthBar), "_creature"));
	/// <summary>读取 <c>NHealthBar</c> 上绑定的 <see cref="Creature"/>（私有字段 <c>_creature</c>）。</summary>
	private static MegaLabel? GetHpLabel(NHealthBar bar) =>
		HpLabelProp.Value?.GetValue(bar) as MegaLabel;
	
	/// <summary>读取 <c>NHealthBar</c> 上绑定的 <see cref="Creature"/>（私有字段 <c>_creature</c>）。</summary>
	private static Creature? GetCreature(NHealthBar bar) =>
		CreatureProp.Value?.GetValue(bar) as Creature;
	#endregion

	#region 病态条相关字段属性
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


	/// <summary>取末日条作克隆模板，布局与原版末日前景一致。</summary>
	private static Control? GetDoomForeground(NHealthBar bar) =>
		AccessTools.Field(typeof(NHealthBar), "_doomForeground")?.GetValue(bar) as Control;

	#endregion

	#region 病态条相关方法
	/// <summary>
	/// 在原版 <c>_Ready</c> 取完节点之后执行：复制末日条、改色为粉红、插入到末日条之后并登记弱表。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(nameof(NHealthBar._Ready))]
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
	[HarmonyPatch("RefreshForeground")]
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
	#endregion

	#region 血条相关方法
	/// <summary>
	/// 在原版 <c>RefreshText</c> 之后执行：更新血条的文本显示；
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch("RefreshText")]
	public static void RefreshText_Postfix(NHealthBar __instance)
	{
		// 获取血条的文本标签和绑定的生物
		var hpLabel = GetHpLabel(__instance);
		var creature = GetCreature(__instance);
		if (hpLabel == null || creature == null)
			return;
		// 死亡、无限血与原版一致：不显示毒/末日，病态条也隐藏
		if (creature.CurrentHp <= 0)
        {
            return;
        }
		// 无限血与原版一致：不显示毒/末日，病态条也隐藏
        if (creature.ShowsInfiniteHp)
        {
            return;
        }
		// 计算病态伤害
		int morbidDamage = creature.GetPower<MorbidPower>()?.CalculateTotalDamageNextTurn() ?? 0;
		// 如果病态伤害致命，则设置血条颜色
		if (IsMorbidLethal(creature, morbidDamage))
		{
			hpLabel.AddThemeColorOverride("font_color", new Color("E27296"));
			hpLabel.AddThemeColorOverride("font_outline_color", new Color("8A1D40"));
		}
		// 如果是一果，显示血量为当前值-1
		if(creature.Monster is Itsuka){
			if(creature.CurrentHp <= 1){
				hpLabel.SetTextAutoSize("累了");
			}else{
				hpLabel.SetTextAutoSize($"{creature.CurrentHp - 1}/{creature.MaxHp-1}");
			}
		}
	}
 	private static bool IsMorbidLethal(Creature creature, int morbidDamage)
    {
        if (morbidDamage <= 0 || !creature.HasPower<MorbidPower>())
        {
            return false;
        }

        return morbidDamage >= creature.CurrentHp;
    }
	#endregion
}
