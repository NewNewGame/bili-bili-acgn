//****************** 代码文件申明 ***********************
//* 文件：Watermelon(西瓜)
//* 作者：wheat
//* 创建时间：2026/04/27 17:32:20 星期一
//* 描述：当你使用药水时，回复5点生命值。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class Watermelon : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 5m)];

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        // 如果药水不是玩家自己使用的，则返回
        if(potion.Owner != base.Owner)
        {
            return;
        }
        // 如果药水是玩家自己使用的，则回复5点生命值
        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, (int)base.DynamicVars["Amount"].BaseValue);
    }

}

