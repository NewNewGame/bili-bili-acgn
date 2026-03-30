namespace CardEditor.Shared.CardPlayActionEmit;

/// <summary>
/// 编辑器内卡牌目标类型（与模组生成代码中的 <c>TargetType.XXX</c> 成员名一致，不引用 sts2）。
/// </summary>
public enum TargetType
{
    None,
    Self,
    AnyEnemy,
    AllEnemies,
    RandomEnemy,
    AnyPlayer,
    AnyAlly,
    AllAllies,
    TargetedNoCreature,
    Osty,
}
