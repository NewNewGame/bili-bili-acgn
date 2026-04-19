//****************** 代码文件申明 ***********************
//* 文件：FunTuan(泛团)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：获得1/2点能量。从消耗牌堆选择2张牌加入你的手牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class FunTuan : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new CardsVar(2),
    ];

    public FunTuan() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
        // 从消耗牌堆选择卡加入手牌
        int num = Mathf.Min((int)base.DynamicVars.Cards.BaseValue, PileType.Exhaust.GetPile(base.Owner).Cards.Count);
        if(num == 0)
        {
            return;
        }
        var cards = (from c in PileType.Exhaust.GetPile(base.Owner).Cards
				orderby c.Rarity, c.Id
                select c).ToList();
        if(cards.Count > 0){
            // 多选 UI 从消耗堆取牌入手牌
            await CardPileCmd.Add(await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, num)), PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
