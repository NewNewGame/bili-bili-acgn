//****************** 代码文件申明 ***********************
//* GuiGui
//* 作者：wheat
//* 创建时间：2026/04/17 星期五
//* 描述：龟怪物模型
//*******************************************************

using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;

public sealed class GuiGui : MonsterBaseModel
{
	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 120, 160);
    public override int MaxInitialHp => MinInitialHp;
    /// <summary>
    /// 重拳伤害
    /// </summary>
	private int StrongPunchDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 14);
    /// <summary>
    /// 防御值
    /// </summary>
	private int BlockValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 10);
	/// <summary>
    /// buff值
    /// </summary>
	private int PowerValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    /// <summary>
    /// 受到伤害音效类型
    /// </summary>
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Stone;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await CreatureCmd.GainBlock(base.Creature, BlockValue, ValueProp.Unpowered, null);
	}
    /// <summary>
    /// 生成怪物逻辑行为状态机
    /// 施加buff -> 重拳出击 -> 多段轻拳 -> 循环 （必须循环）
    /// </summary>
    /// <returns></returns>
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = new List<MonsterState>();
        // 施加buff
		MoveState moveState = new MoveState("READY_MOVE", ReadyMove, new BuffIntent());
        // 重拳出击
		MoveState moveState2 = new MoveState("STRONG_PUNCH_MOVE", StrongPunchMove, new SingleAttackIntent(StrongPunchDamage));
        // 防御
		MoveState moveState3 = new MoveState("BLOCK_MOVE", BlockMove, new DefendIntent(), new DebuffIntent());
        // BUFF -> 重拳出击 -> 防御 -> 循环
		moveState.FollowUpState = moveState2;
		moveState2.FollowUpState = moveState3;
		moveState3.FollowUpState = moveState;
		list.Add(moveState);
		list.Add(moveState3);
		list.Add(moveState2);
		return new MonsterMoveStateMachine(list,  moveState2);
    }
    /// <summary>
    /// 施加BUFF
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    private async Task ReadyMove(IReadOnlyList<Creature> targets)
	{
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_buff");
		await CreatureCmd.TriggerAnim(base.Creature, "Cast", 0.8f);
		await PowerCmd.Apply<StrengthPower>(base.Creature, PowerValue, base.Creature, null);
		await PowerCmd.Apply<PlatingPower>(base.Creature, PowerValue + 2m, base.Creature, null);
	}

    /// <summary>
    /// 重拳出击
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task StrongPunchMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(StrongPunchDamage).FromMonster(this).WithAttackerAnim("Attack", 0.25f)
			.WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_single")
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(null);
	}
    /// <summary>
    /// 多段轻拳
    /// 施加虚弱buff
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task BlockMove(IReadOnlyList<Creature> targets)
	{
		await CreatureCmd.GainBlock(base.Creature, BlockValue, ValueProp.Move, null);
	}

	public override CreatureAnimator GenerateAnimator(MegaSprite controller)
	{
		AnimState animState = new AnimState("idle_loop", isLooping: true);
		AnimState animState2 = new AnimState("attack_double");
		AnimState animState3 = new AnimState("block");
		AnimState animState4 = new AnimState("attack");
		AnimState animState5 = new AnimState("hurt");
		AnimState state = new AnimState("die");
		animState3.NextState = animState;
		animState4.NextState = animState;
		animState5.NextState = animState;
		animState2.NextState = animState;
		CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
		creatureAnimator.AddAnyState("Cast", animState3);
		creatureAnimator.AddAnyState("Attack", animState4);
		creatureAnimator.AddAnyState("Dead", state);
		creatureAnimator.AddAnyState("Hit", animState5);
		creatureAnimator.AddAnyState("DoubleAttack", animState2);
		return creatureAnimator;
	}
}