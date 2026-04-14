//****************** 代码文件申明 ***********************
//* 文件：MangaArtistKirito(漫画家桐人)
//* 作者：wheat
//* 创建时间：2026/04/11
//* 描述：生成1个[gold]攻击[/gold]充能球。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class MangaArtistKirito : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<AttackOrb>()
    ];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Channeling", 1m)];


    public MangaArtistKirito() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int cnt = (int)base.DynamicVars["Channeling"].BaseValue;
        for(int i = 0; i < cnt; i++){
            await OrbCmd.Channel<AttackOrb>(choiceContext, base.Owner);
            if(i < cnt - 1)
            {
                await OrbUtils.OrbChannelingWait();
            }
        }
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars["Channeling"].UpgradeValueBy(1m);
    }
}
