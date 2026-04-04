//****************** 代码文件申明 ***********************
//* 文件：AhaXiuMaker(啊哈！修maker)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：对所有敌人造成{Damage:diff()}点伤害X次。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class AhaXiuMaker : CardBaseModel
{
    #region 卡牌属性配置
    private const int energyCost = -1;
    protected override bool HasEnergyCostX => true;

    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
    ];

    public AhaXiuMaker() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 动画效果
        int num = ResolveEnergyXValue();
        if(num <= 0) return;

        // 播放动画效果
		Color color = new Color("FFFFFF80");
		double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
		NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + (double)Mathf.Min(8, num) * num2));
		SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
		NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
        // 造成伤害 X 次
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .WithHitCount(num)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级效果 伤害增加1点，获得[gold]有一说一[/gold]
        base.DynamicVars["Damage"].UpgradeValueBy(1m);
        base.AddKeyword(CustomKeyWords.YYSY);
    }
}
