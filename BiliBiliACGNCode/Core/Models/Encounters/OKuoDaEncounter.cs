//****************** 代码文件申明 ***********************
//* OKuoDaEncounter
//* 作者：wheat
//* 创建时间：2026/04/02 15:09:02 星期四
//* 描述：Pop子与Pipi美战斗场景
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;

public sealed class OKuoDaEncounter : EncounterModel
{
	public override RoomType RoomType => RoomType.Monster;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<PopPipi>()];
	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		PopPipi popPipi = (PopPipi)ModelDb.Monster<PopPipi>().ToMutable();
		popPipi.StartsWithStrongPunch = true;
		popPipi.StartingHpReduction = base.Rng.NextInt(2, 10);
		return [
			new (popPipi, null),
		];
	}
}
