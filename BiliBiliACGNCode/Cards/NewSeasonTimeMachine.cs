//****************** 代码文件申明 ***********************
//* 文件：NewSeasonTimeMachine(新番时光机)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：[gold]激发[/gold]你最右侧的充能球{Times:diff()}次，然后复制[gold]生成[/gold]1个刚才被[gold]激发[/gold]的充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class NewSeasonTimeMachine : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.Static(StaticHoverTip.Evoke)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Times", 1m),
    ];

    public NewSeasonTimeMachine() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var rightOrb = base.Owner.PlayerCombatState?.OrbQueue.Orbs.First();
        if(rightOrb == null) return;
        int times = base.DynamicVars["Times"].IntValue;
        // [gold]激发[/gold]你最右侧的充能球{Times:diff()}次，然后复制[gold]生成[/gold]1个刚才被[gold]激发[/gold]的充能球。
        for(int i = 0; i < times; i++)
        {
            await OrbCmd.EvokeNext(choiceContext, base.Owner, i == times -1);
            await OrbUtils.OrbEvokeWait();
        }
        // 复制[gold]生成[/gold]1个刚才被[gold]激发[/gold]的充能球。
        await OrbCmd.Channel(choiceContext, rightOrb, base.Owner);

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Times"].UpgradeValueBy(1m);
    }
}
