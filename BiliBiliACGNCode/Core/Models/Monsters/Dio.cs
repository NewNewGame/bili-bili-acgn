//****************** 代码文件申明 ***********************
//* Dio
//* 作者：wheat
//* 创建时间：2026/05/04 21:00:00 星期一
//* 描述：Dio 怪物模型
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
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

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;

public sealed class Dio : MonsterBaseModel
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 250, 200);
    public override int MaxInitialHp => MinInitialHp;
    /// <summary>
    /// 嗜血啃咬伤害
    /// </summary>
	private int BloodBiteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 9);
    /// <summary>
    /// 连击伤害
    /// </summary>
	private int MudaMudaAttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);
    /// <summary>
    /// 连击次数
    /// </summary>
    private int MudaMudaAttackCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 4);
	/// <summary>
    /// buff值
    /// </summary>
	private int PowerValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);
    /// <summary>
    /// 世界值
    /// </summary>
	private int WorldValue => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 6);
    /// <summary>
    /// 受到伤害音效类型
    /// </summary>
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Magic;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
	}
    /// <summary>
    /// 生成怪物逻辑行为状态机
    /// 施加buff -> 重拳出击 -> 多段轻拳 -> 循环 （必须循环）
    /// </summary>
    /// <returns></returns>
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = new List<MonsterState>();
        // 施加Debuff
		MoveState moveState = new MoveState("READY_MOVE", ReadyMove, new DebuffIntent());
        // 嗜血啃咬
		MoveState moveState2 = new MoveState("BLOOD_BITE_MOVE", BloodBiteMove, new SingleAttackIntent(BloodBiteDamage));
        // 连击
		MoveState moveState3 = new MoveState("MUDAMUDA_ATTACK_MOVE", MudaMudaAttackMove, new MultiAttackIntent(MudaMudaAttackDamage, MudaMudaAttackCount));
        // 施加buff
		MoveState moveState4 = new MoveState("BUFF_MOVE", BuffMove, new BuffIntent());
        // 施加Debuff -> 嗜血啃咬 -> 连击 -> 施加buff -> 嗜血啃咬循环
		moveState.FollowUpState = moveState2;
		moveState2.FollowUpState = moveState3;
		moveState3.FollowUpState = moveState4;
		moveState4.FollowUpState = moveState2;
		list.Add(moveState);
		list.Add(moveState2);
		list.Add(moveState3);
		list.Add(moveState4);
		return new MonsterMoveStateMachine(list,  moveState);
    }
    /// <summary>
    /// 施加BUFF
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    private async Task ReadyMove(IReadOnlyList<Creature> targets)
	{
        // 播放The World音效
		SfxCmd.Play(AudioUtils.TheWorldEventPath);
		await CreatureCmd.TriggerAnim(base.Creature, "Cast", 2f);
        // 给予所有玩家世界
        foreach(var player in base.CombatState.Players){
            await PowerCmd.Apply<TheWorldPower>(player.Creature, WorldValue, base.Creature, null);
        }
	}
    /// <summary>
    /// 施加buff
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task BuffMove(IReadOnlyList<Creature> targets)
	{
		await PowerCmd.Apply<StrengthPower>(base.Creature, PowerValue, base.Creature, null);
	}

    /// <summary>
    /// 重拳出击
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task BloodBiteMove(IReadOnlyList<Creature> targets)
	{
		var dmgCommand = await DamageCmd.Attack(BloodBiteDamage).FromMonster(this).WithAttackerAnim("Attack", 0.25f)
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(null);
        int heal = 0;
        foreach(var result in dmgCommand.Results){
            heal += result.UnblockedDamage;
        }
        if(heal > 0){
            // 治疗
            await CreatureCmd.Heal(base.Creature, heal);
        }
	}
    /// <summary>
    /// 多段轻拳
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
	private async Task MudaMudaAttackMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(MudaMudaAttackDamage).WithHitCount(MudaMudaAttackCount).FromMonster(this)
			.WithAttackerAnim("DoubleAttack", 0.2f)
			.OnlyPlayAnimOnce()
			.WithHitFx("vfx/vfx_attack_blunt")
			.Execute(null);
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