//****************** 代码文件申明 ***********************
//* 文件：ModConfig
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:23 星期四
//* 描述：Mod配置文件，用于配置Mod的选项
//*******************************************************
using BaseLib.Config;

namespace BiliBiliACGN.BiliBiliACGNCode.Core;

public class ModConfig : SimpleModConfig
{
    public static bool Test1 { get; set; } = true;
}