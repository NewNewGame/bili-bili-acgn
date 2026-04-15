//****************** 代码文件申明 ***********************
//* 文件：NiulangVillage(牛郎村)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：X费。获得{IfUpgraded:show:X+1|X}点[gold]力量[/gold]。\n给予所有敌人{IfUpgraded:show:X+1|X}层[gold]易伤[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NiulangVillage : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = -1;
    protected override bool HasEnergyCostX => true;

    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    public NiulangVillage() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 计算X，升级+1
        int x = ResolveEnergyXValue();
        if(base.IsUpgraded) ++x;
        // 获得力量
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, x, base.Owner.Creature, this);
        // 给予所有敌人易伤
        if(base.CombatState == null) return;
        foreach(var enemy in base.CombatState.HittableEnemies){
            await PowerCmd.Apply<VulnerablePower>(enemy, x, base.Owner.Creature, this);
        }
    }

}
