//****************** 代码文件申明 ***********************
//* 文件：Penguin(企鹅)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：消耗你的所有手牌，每消耗1张牌，生成1个随机充能球，并抽1张牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Penguin : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public Penguin() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 消耗手牌中所有牌
        List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        // 每消耗 1 张，生成 1 个随机充能球，并抽 1 张牌
        for(int i = 0; i < list.Count; i++)
        {
            await CardCmd.Exhaust(choiceContext, list[i]);
            await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(base.Owner.RunState.Rng.CombatOrbGeneration), base.Owner);
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
