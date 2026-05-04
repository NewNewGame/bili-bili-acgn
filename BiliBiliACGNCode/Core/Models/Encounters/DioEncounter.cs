//****************** 代码文件申明 ***********************
//* DioEncounter
//* 作者：wheat
//* 创建时间：2026/05/04 21:00:00 星期一
//* 描述：Dio 战斗场景
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;

public sealed class DioEncounter : EncounterModel
{
	public override RoomType RoomType => RoomType.Monster;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Dio>()];
	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		Dio dio = (Dio)ModelDb.Monster<Dio>().ToMutable();
		return [
			new (dio, null),
		];
	}
}