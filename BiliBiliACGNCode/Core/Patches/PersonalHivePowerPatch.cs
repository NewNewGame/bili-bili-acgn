//****************** 代码文件申明 ***********************
//* PersonalHivePowerPatch
//* 作者：wheat
//* 创建时间：2026/04/14
//* 描述：人体蜂房Power修复补丁
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(PersonalHivePower))]
public static class PersonalHivePowerPatch
{
	/// <summary>
	/// 如果施加者是宠物，则修正为玩家
	/// </summary>
	[HarmonyPrefix]
	[HarmonyPatch(nameof(PersonalHivePower.AfterDamageReceived))]
	public static void AfterDamageReceived_Prefix(
		PlayerChoiceContext choiceContext,
		Creature target,
		DamageResult _,
		ValueProp props,
		ref Creature? dealer,
		CardModel? cardSource)
	{
        if(dealer == null || dealer.PetOwner == null) return;
		dealer = dealer.PetOwner?.Creature;
	}
}