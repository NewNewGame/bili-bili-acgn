//****************** 代码文件申明 ***********************
//* PureGoldCardEncounter
//* 作者：wheat
//* 创建时间：2026/04/02 15:09:02 星期四
//* 描述：纯金卡牌事件场景
//*******************************************************

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;

public sealed class PureGoldCardEncounter : EncounterModel
{
    public override RoomType RoomType => RoomType.Monster;
    // TODO 暂时先用这个，之后再改
	public override IEnumerable<MonsterModel> AllPossibleMonsters => [];
	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return [
		];
	}
}