//****************** 代码文件申明 ***********************
//* 文件：TrackPlayer(轨迹玩家)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：复制你所有的充能球一份。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class TrackPlayer : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public TrackPlayer() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(base.Owner.PlayerCombatState == null) return;
        // 复制你所有的充能球一份
        var orbs = base.Owner.PlayerCombatState.OrbQueue.Orbs.ToList();
        for(int i = 0; i < orbs.Count; i++)
        {
            await OrbCmd.Channel(choiceContext, orbs[i], base.Owner);
            if(i < orbs.Count - 1)
            {
                await OrbUtils.OrbChannelingWait();
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级移除消耗
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}
