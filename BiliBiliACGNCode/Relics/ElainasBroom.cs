//****************** 代码文件申明 ***********************
//* 文件：ElainasBroom
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：伊蕾娜的扫帚 可以在楼层中飞行
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Patches;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class ElainasBroom : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override Task AfterObtained()
    {
        // 获得伊蕾娜的扫帚时，重新刷新地图
        NMapScreenRecalculateTravelabilityPatch.TryRefreshMap();
        return base.AfterObtained();
    }
}
