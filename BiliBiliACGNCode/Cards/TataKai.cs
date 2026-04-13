//****************** 代码文件申明 ***********************
//* 文件：TataKai(塔塔开)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：女儿向敌人发动{Hits:diff()}次[gold]进攻[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class TataKai : CardBaseModel
{
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Hits", 3m)
    ];

    public TataKai() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 女儿向敌人发动{Hits:diff()}次[gold]进攻[/gold]。
        int hits = base.DynamicVars["Hits"].IntValue;
        for(int i = 0; i < hits; i++)
        {
            await DaughterCmd.ApplyAttack(base.Owner.Creature, 0, choiceContext, cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Hits"].UpgradeValueBy(1m);
    }
}
