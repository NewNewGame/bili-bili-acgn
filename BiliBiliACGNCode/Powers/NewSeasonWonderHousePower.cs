//****************** 代码文件申明 ***********************
//* 文件：NewSeasonWonderHousePower(新番妙妙屋)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当你打出能力牌时，获得 Amount 点能量
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NewSeasonWonderHousePower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // TODO: AfterCardPlayed 检测能力牌，PlayerCmd.GainEnergy
}
