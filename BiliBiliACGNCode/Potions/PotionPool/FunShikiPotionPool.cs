//****************** 代码文件申明 ***********************
//* 文件：FunShikiPotionPool
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：泛式药水池
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Potions.PotionPool;

public sealed class FunShikiPotionPool : CustomPotionPoolModel
{
    public override string? TextEnergyIconPath{
        get
        {
            var path = $"funshiki_energy_icon.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override string? BigEnergyIconPath {
        get
        {
            var path = $"funshiki_energy_big.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
}