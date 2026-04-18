//****************** 代码文件申明 ***********************
//* 文件：NotAfraidOfMyLies(他不怕我说谎吗)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：若敌人意图为攻击，获得{Power:diff()}点[gold]唐氏[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NotAfraidOfMyLies : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TangShiPower>()];
    // 敌人意图为攻击时高亮
    protected override bool ShouldGlowGoldInternal
	{
		get
		{
			if (base.CombatState == null)
			{
				return false;
			}
			return base.CombatState.HittableEnemies.Any((Creature e) => e.Monster?.IntendsToAttack ?? false);
		}
	}
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 3m)
    ];

    public NotAfraidOfMyLies() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 若敌人意图为攻击，获得{Power:diff()}层唐氏
        if(cardPlay.Target.Monster?.IntendsToAttack ?? false){
            await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Power"].UpgradeValueBy(2m);
    }
}
