//****************** 代码文件申明 ***********************
//* 文件：EiHeiJiangMask
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：欸嘿酱的面具 战斗开始时，召唤一果进行助战
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;


[Pool(typeof(SharedRelicPool))]
public sealed class EiHeiJiangMask : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStart()
	{
		await SummonPet();
	}

	private async Task SummonPet()
	{
		await DaughterCmd.SummonDaughter(base.Owner.Creature);
	}

}
