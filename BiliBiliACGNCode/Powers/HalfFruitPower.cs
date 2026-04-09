//****************** 代码文件申明 ***********************
//* 文件：HalfFruitPower(0.5果)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：每当女儿攻击敌人时，这名敌人在本回合变态。
//*******************************************************

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class HalfFruitPower : PowerBaseModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GetTangPower>()];

    /// <summary>
    /// 每当女儿攻击敌人时，这名敌人在本回合失去力量。
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="dealer"></param>
    /// <param name="result"></param>
    /// <param name="props"></param>
    /// <param name="target"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if(dealer != base.Owner || target == null) return;
        // 使目标本回合变唐
        await PowerCmd.Apply<GetTangPower>(target, Amount, dealer, null);
    }

}
