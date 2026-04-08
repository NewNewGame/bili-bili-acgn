//****************** 代码文件申明 ***********************
//* 文件：OrbUtils
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：充能球工具类
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models;
using Rng = MegaCrit.Sts2.Core.Random.Rng;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class OrbUtils
{
    /// <summary>
    /// 泛式充能球
    /// </summary>
    public static IReadOnlyCollection<OrbModel> FunShikiOrbs =>[
        ModelDb.Orb<AttackOrb>(),
        ModelDb.Orb<StrengthOrb>(),
        ModelDb.Orb<BlockOrb>(),
    ];
    /// <summary>
    /// 随机获取一个泛式充能球
    /// </summary>
    /// <param name="rng">随机数生成器</param>
    /// <returns>泛式充能球</returns>
    public static OrbModel GetRandomFunShikiOrb(Rng rng)
    {
        return rng.NextItem(FunShikiOrbs);
    }
}   