//****************** 代码文件申明 ***********************
//* 文件：WindRangerMask
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：旋风游侠面具 战斗开始时，召唤一果进行助战，其他待补充
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;


[Pool(typeof(FunShikiRelicPool))]
public sealed class WindRangerMask : RelicBaseModel
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