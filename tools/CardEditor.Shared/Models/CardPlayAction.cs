using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

/// <summary>
/// 打出卡牌后执行的一条效果（顺序与列表一致）。转 C# 代码的规则后续单独实现。
/// </summary>
public sealed class CardPlayAction
{
    /// <summary>
    /// 操作类型：DrawCards 抽牌、Damage 造成伤害、Discard 弃牌、Exhaust 消耗牌等。
    /// </summary>
    [JsonPropertyName("actionType")]
    public string ActionType { get; set; } = "DrawCards";

    /// <summary>
    /// 数值来源：<c>literal</c> 使用 <see cref="Value"/>；否则为 <see cref="DynamicVarEntry.Kind"/>，与动态变量表中同名字段对应（取第一条匹配）。
    /// </summary>
    [JsonPropertyName("valueBinding")]
    public string ValueBinding { get; set; } = "literal";

    /// <summary>
    /// 当 <see cref="ValueBinding"/> 为 <c>literal</c> 时的固定数值；绑定变量时仍可保留作占位或说明。
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// 编辑器备注，不参与运行时逻辑。
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
