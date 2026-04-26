//****************** 代码文件申明 ***********************
//* 文件：Dcm(DCM)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：造成{Damage:diff()}点伤害。\n给予{Weak:diff()}层[gold]虚弱[/gold]。\n[gold]生成[/gold]{StrengthOrb:diff()}个[gold]力量[/gold]充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Dcm : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<StrengthOrb>(),
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("StrengthOrb", 1m)
    ];

    public Dcm() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对目标敌人造成伤害，给予{Weak:diff()}层虚弱
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars["Weak"].BaseValue, base.Owner.Creature, this);
        // 生成{StrengthOrb:diff()}个力量充能球
        int cnt = (int)base.DynamicVars["StrengthOrb"].BaseValue;
        for(int i = 0; i < cnt; i++){
            await OrbCmd.Channel<StrengthOrb>(choiceContext, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
        base.DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}
