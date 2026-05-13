//****************** 代码文件申明 ***********************
//* 文件：Transference(移情别恋)
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：造成7/9点伤害。给予其他敌人该名敌人身上所有的负面效果。
//*******************************************************


using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(FunShikiCardPool))]
public sealed class Transference : CardBaseModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
    ];

    public Transference() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害；将目标身上所有负面效果复制给其它敌人
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);

        // 如果只有1个敌人那就返回
        if(base.CombatState?.HittableEnemies.Count() == 1) return;
		List<PowerModel> originalDebuffs = (from p in cardPlay.Target.Powers
			where p.TypeForCurrentAmount == PowerType.Debuff
			select (PowerModel)p.ClonePreservingMutability()).ToList();
        // 将目标身上所有负面效果复制给其它敌人
		foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            if (enemy == cardPlay.Target)
            {
                continue;
            }

            foreach (PowerModel item in originalDebuffs)
            {
                PowerModel powerModel = PowerCmd.FindExistingInstanceForStacking(item, enemy, item.Applier);
                if (powerModel != null)
                {
                    DoHackyThingsForSpecificPowers(powerModel);
                    await PowerCmd.ModifyAmount(choiceContext, powerModel, item.Amount, item.Applier, this);
                }
                else
                {
                    PowerModel power = (PowerModel)item.ClonePreservingMutability();
                    DoHackyThingsForSpecificPowers(power);
                    await PowerCmd.Apply(choiceContext, power, enemy, item.Amount, item.Applier, this);
                }
            }
        }
    }

    private void DoHackyThingsForSpecificPowers(PowerModel power)
    {
        if (power is ITemporaryPower temporaryPower)
		{
			temporaryPower.IgnoreNextInstance();
		}
    }


    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.AddKeyword(CardKeyword.Retain);
    }
}
