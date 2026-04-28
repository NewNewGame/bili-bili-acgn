//****************** 代码文件申明 ***********************
//* 文件：AloftThrone(高高在上)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：失去1个充能球栏位。\n获得{Focus:diff()}点[gold]集中[/gold]。\n获得{Dexterity:diff()}点[gold]敏捷[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class AloftThrone : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Focus", 1m),
        new DynamicVar("Dexterity", 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<FocusPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];

    public AloftThrone() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 失去1个充能球栏位
        OrbCmd.RemoveSlots(base.Owner, 1);
        // 获得集中和敏捷
        await PowerCmd.Apply<FocusPower>(base.Owner.Creature, base.DynamicVars["Focus"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars["Dexterity"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Focus"].UpgradeValueBy(1m);
        base.DynamicVars["Dexterity"].UpgradeValueBy(1m);
    }
}
