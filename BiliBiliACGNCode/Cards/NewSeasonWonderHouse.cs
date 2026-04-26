//****************** 代码文件申明 ***********************
//* 文件：NewSeasonWonderHouse(新番妙妙屋)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：能力：每当你打出1张能力牌时，获得1点能量。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class NewSeasonWonderHouse : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("OrbCount", 1m),
    ];

    public NewSeasonWonderHouse() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 添加新番妙妙屋BUFF
        await PowerCmd.Apply<NewSeasonWonderHousePower>(base.Owner.Creature, base.DynamicVars["OrbCount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 添加固有
        base.AddKeyword(CardKeyword.Innate);
    }
}
