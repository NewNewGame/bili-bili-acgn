//****************** 代码文件申明 ***********************
//* 文件：ModConfig
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:23 星期四
//* 描述：Mod配置文件，用于配置Mod的选项
//*******************************************************
using BaseLib.Config;

namespace BiliBiliACGN.BiliBiliACGNCode.Core;

[HoverTipsByDefault]
public sealed class ACGNModConfig : SimpleModConfig
{
    /// <summary>
    /// 是否使用角色语音
    /// </summary>
    [ConfigSection("启用角色语音")]
    public static bool UseCharacterVoice { get; set; } = false;
}