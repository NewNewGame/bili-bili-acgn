//****************** 代码文件申明 ***********************
//* 文件：FanshikiCardPool
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：泛式卡池
//*******************************************************

using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;


public sealed class FanshikiCardPool : CustomCardPoolModel
{
    public override string Title => "Fanshiki";

    public override string EnergyColorName => CustomEnergyIconPatches.GetEnergyColorName(Id);
    public override string? TextEnergyIconPath{
        get
        {
            var path = $"fanshiki_energy_icon.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override string? BigEnergyIconPath {
        get
        {
            var path = $"fanshiki_energy_big.png".EnergyIconImagePath();
            return ResourceLoader.Exists(path) ? path : "colorless_energy_icon.png".EnergyIconImagePath();
        }
    }
    public override Color ShaderColor => new Color("E02D16");
    public override Color DeckEntryCardColor => new Color("E02D16");
    public override Color EnergyOutlineColor => new Color("751207");

    public override bool IsColorless => false;
    public override bool IsShared => true;

    protected override CardModel[] GenerateAllCards()
    {
        // 所有[Pool(typeof(FanshikiCardPool))]的卡牌
        return [

        ];
    }
}
