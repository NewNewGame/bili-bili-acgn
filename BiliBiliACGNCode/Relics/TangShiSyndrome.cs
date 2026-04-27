//****************** 代码文件申明 ***********************
//* 文件：TangShiSyndrome(唐氏综合征)
//* 作者：wheat
//* 创建时间：2026/04/27 09:34:00 星期一
//* 描述：在你的回合开始时，获得1点唐氏，并给予所有敌人1层变唐。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class TangShiSyndrome : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<TangShiPower>(),
        HoverTipFactory.FromPower<GetTangPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Tang", 1m),
        new DynamicVar("GetTang", 1m),
    ];
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 如果对象不是自己，或者战斗状态为空，则返回
        if (player != base.Owner || player.Creature.CombatState == null)
        {
            return;
        }
        // 闪烁
        Flash();
        await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, base.DynamicVars["Tang"].BaseValue, base.Owner.Creature, null);
        // 给予所有敌人1层变唐
        foreach (var enemy in player.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<GetTangPower>(enemy, base.DynamicVars["GetTang"].BaseValue, base.Owner.Creature, null);
        }
    }

}

