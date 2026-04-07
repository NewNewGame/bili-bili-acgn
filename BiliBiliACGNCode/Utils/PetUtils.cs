//****************** 代码文件申明 ***********************
//* 文件：PetUtils
//* 作者：wheat
//* 创建时间：2026/04/07 10:00:00 星期二
//* 描述：宠物辅助类
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class PetUtils
{
    /// <summary>
    /// 获取宠物
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static Creature? GetPet<T>(this Creature creature) where T : MonsterModel{
        foreach (var pet in creature.Pets)
        {
            if (pet.Monster is T)
            {
                return pet;
            }
        }
        return null;
    }
    /// <summary>
    /// 判断有没有指定宠物
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="creature"></param>
    /// <returns></returns>
    public static bool HasPet<T>(this Creature creature) where T : MonsterModel{
        return creature.Pets.Any(pet => pet.Monster is T);
    }
}