//****************** 代码文件申明 ***********************
//* 文件：UtterMadness(彻底疯狂)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：女儿向敌人发动进攻，次数等同于你当前充能球的{OrbCountMode:diff()}。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class UtterMadness : CardBaseModel
{
    #region 卡牌关键词与悬停

    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public UtterMadness() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 女儿向敌人发动{Hits:diff()}次[gold]进攻[/gold]。
        int num = base.IsUpgraded ? base.Owner.PlayerCombatState.OrbQueue.Orbs.Count() : (from orb in base.Owner.PlayerCombatState.OrbQueue.Orbs
				group orb by orb.Id).Count();
        for(int i = 0; i < num; i++)
        {
            await DaughterCmd.ApplyAttack(base.Owner.Creature, 0, choiceContext, cardPlay.Target);
            await Cmd.Wait(0.25f);
        }
    }
}
