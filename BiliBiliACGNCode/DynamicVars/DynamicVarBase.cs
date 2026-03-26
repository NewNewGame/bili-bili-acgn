using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.DynamicVars;

public abstract class DynamicVarBase : DynamicVar
{
    public static string Key => nameof(DynamicVarBase);
    public static readonly string LocKey = Key.ToUpperInvariant();
    
    public DynamicVarBase(decimal baseValue) : base(Key, baseValue)
    {
        // 添加词条提示
        this.WithTooltip(LocKey);
    }
}