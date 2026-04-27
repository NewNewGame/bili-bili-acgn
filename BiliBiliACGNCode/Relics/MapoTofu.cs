//****************** 代码文件申明 ***********************
//* 文件：MapoTofu(麻婆豆腐)
//* 作者：wheat
//* 创建时间：2026/04/27 17:31:20 星期一
//* 描述：拾起时，回复10点生命值。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MapoTofu : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 10m)];
    public override async Task AfterObtained()
    {
        await CreatureCmd.Heal(base.Owner.Creature, (int)base.DynamicVars["Amount"].BaseValue);
    }
}

