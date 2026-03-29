namespace CardEditorGui;

/// <summary>
/// CardPlayAction「数值来源」下拉项：Key 写入 JSON（literal 或 DynamicVar 的 kind）。
/// </summary>
public sealed class ValueBindingOption
{
    public required string Key { get; init; }
    public required string Display { get; init; }
}
