//****************** 代码文件申明 ***********************
//* 文件：MoWaLuMoWaLu(抹瓦鲁抹瓦鲁)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：给予7/11层病态。当这名敌人死亡时，对其他敌人造成等同于它最大生命值的伤害。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class MoWaLuMoWaLu : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Morbid", 7m),
    ];

    public MoWaLuMoWaLu() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 施加病态，并设置“死亡时按最大生命值对其它敌人造成伤害”的效果
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Morbid"].UpgradeValueBy(4m);
    }
}
