//****************** 代码文件申明 ***********************
//* 文件：FunShikiEcstasy(泛式狂喜)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：向敌人施加病态5点X/X+1次。
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
public sealed class FunShikiEcstasy : CardBaseModel
{
    private const int energyCost = -1;
    protected override bool HasEnergyCostX => true;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MorbidPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MorbidAmount", 5m),
    ];

    public FunShikiEcstasy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target == null){
            return;
        }
        int num = ResolveEnergyXValue();
        if(base.IsUpgraded)++num;
        for(int i = 0; i < num; i++){
            await PowerCmd.Apply<MorbidPower>(cardPlay.Target, base.DynamicVars["MorbidAmount"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() { }
}
