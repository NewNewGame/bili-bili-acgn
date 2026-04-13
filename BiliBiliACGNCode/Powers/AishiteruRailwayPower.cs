//****************** 代码文件申明 ***********************
//* 文件：AishiteruRailwayPower(爱洗铁路)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每打出1张能力牌，对随机敌人施加 Amount 层病态。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AishiteruRailwayPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];

    /// <summary>
    /// Amount：每次打出能力牌时，向随机敌人叠加的病态层数（由卡牌 <see cref="AishiteruRailway"/> 施加时写入）。
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner.Player || cardPlay.Card.Type != CardType.Power)
        {
            return;
        }

        if (base.Owner.CombatState == null)
        {
            return;
        }

        var enemies = base.Owner.CombatState.HittableEnemies;
        if (enemies.Count == 0)
        {
            return;
        }

        var target = base.Owner.CombatState.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null)
        {
            return;
        }

        await PowerCmd.Apply<MorbidPower>(target, Amount, base.Owner, null);
    }
}
