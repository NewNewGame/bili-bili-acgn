//****************** 代码文件申明 ***********************
//* 文件：Vector(Vector)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：造成等量于敌人身上病态层数的伤害。（升级后添加保留）
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Vector : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public Vector() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 造成等同于目标“病态”层数的伤害
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        // TODO: 如设计需要：升级后为本牌添加保留（Retain）关键词
    }
}
