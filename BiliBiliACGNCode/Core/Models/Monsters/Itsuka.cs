//****************** 代码文件申明 ***********************
//* Itsuka
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：一果Model
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Monsters;
public sealed class Itsuka : MonsterBaseModel
{
    public override int MinInitialHp => 10;

    public override int MaxInitialHp => 10;

    public override bool IsHealthBarVisible => base.Creature.IsAlive;

    /* 皮肤暂时不打算做
    public override void SetupSkins(NCreatureVisuals visuals)
    {
        string skinName = (!base.IsMutable) ? Itsuka.SkinOptions[0] : base.Creature.PetOwner.GetRelic<EiHeiJiangMask>().Skin;
        MegaSkeleton skeleton = visuals.SpineBody.GetSkeleton();
        MegaSkeletonDataResource data = skeleton.GetData();
        skeleton.SetSkin(data.FindSkin(skinName));
        skeleton.SetSlotsToSetupPose();
    }
    */
    /// <summary>
    /// 状态机生成
    /// 挂机
    /// </summary>
    /// <returns></returns>
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        moveState.FollowUpState = moveState;
        return new MonsterMoveStateMachine(new List<MonsterState> { moveState }, moveState);
    }
}