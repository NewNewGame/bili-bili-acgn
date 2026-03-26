//****************** 代码文件申明 ***********************
//* 文件：DynamicVarBase
//* 作者：wheat
//* 创建时间：2026/03/26 10:47:02 星期四
//* 描述：动态变量基类（攻击、防御、吸血、中毒）
//*******************************************************
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