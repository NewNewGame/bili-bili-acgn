//****************** 代码文件申明 ***********************
//* 文件：LogUtils
//* 作者：wheat
//* 创建时间：2026/04/09
//* 描述：日志工具类
//*******************************************************

using MegaCrit.Sts2.Core.Logging;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class LogUtils
{
    public static string ModPrefix = "[B站动画区Mod] ";
    public static void LogInfo(this string message){
        Log.Info($"{ModPrefix} {message}");
    }
}