//****************** 代码文件申明 ***********************
//* 文件：FinaleRoast(完结吐槽)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：获得{Block:diff()}点[gold]格挡[/gold]。[gold]激发[/gold]{Evokes:diff()}个充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class FinaleRoast : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block),HoverTipFactory.Static(StaticHoverTip.Evoke)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new DynamicVar("Evokes", 2m)
    ];

    public FinaleRoast() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得{Block:diff()}点[gold]格挡[/gold]。[gold]激发[/gold]{Evokes:diff()}个充能球。
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, base.DynamicVars.Block.Props, cardPlay);
        // 如果没有充能球就返回
        if(base.Owner.PlayerCombatState?.OrbQueue.Orbs.Count == 0) return;
        int cnt = (int)base.DynamicVars["Evokes"].BaseValue;
        for(int i = 0; i < cnt; i++)
        {
            await OrbCmd.EvokeNext(choiceContext, base.Owner);
            if(i < cnt - 1)
            {
                await Cmd.Wait(0.25f);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Block"].UpgradeValueBy(3m);
        base.DynamicVars["Evokes"].UpgradeValueBy(1m);
    }
}
