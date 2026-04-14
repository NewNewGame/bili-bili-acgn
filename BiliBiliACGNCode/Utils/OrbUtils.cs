//****************** 代码文件申明 ***********************
//* 文件：OrbUtils
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：充能球工具类
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using Rng = MegaCrit.Sts2.Core.Random.Rng;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class OrbUtils
{
    /// <summary>
    /// 泛式充能球
    /// </summary>
    public static IReadOnlyCollection<OrbModel> FunShikiOrbs =>[
        ModelDb.Orb<AttackOrb>().ToMutable(),
        ModelDb.Orb<StrengthOrb>().ToMutable(),
        ModelDb.Orb<BlockOrb>().ToMutable(),
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
    /// <summary>
    /// 随机获取一个泛式充能球
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public static OrbModel GetRandomFunShikiOrb(this CardModel card)
    {
        return GetRandomFunShikiOrb(card.CombatState.RunState.Rng.CombatOrbGeneration);
    }
    /// <summary>
    /// 随机获取一个泛式充能球
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public static OrbModel GetRandomFunShikiOrb(this CombatState combatState)
    {
        return GetRandomFunShikiOrb(combatState.RunState.Rng.CombatOrbGeneration);
    }
    /// <summary>
    /// 充能球激发等待
    /// </summary>
    /// <returns></returns>
    public static Task OrbEvokeWait(){
        return Cmd.CustomScaledWait(0.15f, 0.25f);
    }
    /// <summary>
    /// 充能球生成等待
    /// </summary>
    /// <returns></returns>
    public static Task OrbChannelingWait(){
        return Cmd.CustomScaledWait(0.05f, 0.10f);
    }
}   