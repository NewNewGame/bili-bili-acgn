//****************** 代码文件申明 ***********************
//* 文件：Birthday(生日)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：下/本回合，生成2个随机充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Birthday : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Channeling)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("RandomOrbs", 2m),
    ];

    public Birthday() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 生成2个随机充能球
        int num = (int)base.DynamicVars["RandomOrbs"].BaseValue;
        // 如果升级了，则生成2个随机充能球
        if(base.IsUpgraded){
            for(int i = 0; i < num; i++){
                await OrbCmd.Channel(choiceContext, OrbUtils.GetRandomFunShikiOrb(this),base.Owner);
                if(i < num - 1)
                {
                    await OrbUtils.OrbChannelingWait();
                }
            }
        }else{
            // 下回合生成2个随机充能球
            await PowerCmd.Apply<DelayOrbPower>(base.Owner.Creature, num, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
