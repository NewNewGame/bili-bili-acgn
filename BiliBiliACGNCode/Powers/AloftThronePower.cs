//****************** 代码文件申明 ***********************
//* 文件：AloftThronePower(高高在上)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：每当女儿发动进攻时，抽 Amount 张牌
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AloftThronePower : PowerBaseModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(CustomeHoverTips.AttackOrb)];
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每当女儿发动进攻时，抽 Amount 张牌
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if(base.Owner.Player == null) return;
        if(dealer == null || dealer.Monster is not Itsuka) return;
        // 如果不是在出牌阶段，则下回合抽牌
        if(RunManager.Instance.ActionQueueSynchronizer.CombatState != ActionSynchronizerCombatState.PlayPhase){
            await PowerCmd.Apply<DrawCardsNextTurnPower>(base.Owner, base.Amount, base.Owner, null);
        }else{
            await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
        }
    }

}
