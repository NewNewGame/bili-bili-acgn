//****************** 代码文件申明 ***********************
//* PersonalHivePowerPatch
//* 作者：wheat
//* 创建时间：2026/04/14
//* 描述：人体蜂房Power修复补丁
//*******************************************************

using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(PersonalHivePower))]
public static class PersonalHivePowerPatch
{
	/// <summary>
	/// 如果施加者是宠物，则修正为null(看需求)
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
        // 如果施加者为空或宠物没有主人，则返回
        if(dealer == null || dealer.PetOwner == null) return;
        // 如果施加者不是Osty，则修正为null
        if(dealer.Monster is not Osty){
		    dealer = null;
        }
	}
}