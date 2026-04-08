//****************** 代码文件申明 ***********************
//* 文件：FunShikiCardPool
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


public sealed class FunShikiCardPool : CustomCardPoolModel
{
    public override string Title => "FunShiki";

    public override string EnergyColorName => CustomEnergyIconPatches.GetEnergyColorName(Id);
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
    public override Color ShaderColor => new Color("E02D16");
    public override Color DeckEntryCardColor => new Color("E02D16");
    public override Color EnergyOutlineColor => new Color("751207");

    public override bool IsColorless => false;
    public override bool IsShared => true;

    protected override CardModel[] GenerateAllCards()
    {
        // 所有[Pool(typeof(FanshikiCardPool))]的卡牌
        return [
            ModelDb.Card<QianDaJiang>(),
            ModelDb.Card<MawaluMawalu>(),
            ModelDb.Card<ChampionPowder>(),
            ModelDb.Card<FunTuan>(),
            ModelDb.Card<Penguin>(),
            ModelDb.Card<Diary>(),
            ModelDb.Card<TrackPlayer>(),
            ModelDb.Card<OhHoHoHo>(),
            ModelDb.Card<MemeMailBox>(),
            ModelDb.Card<ImSoDespair>(),
            ModelDb.Card<YandereForm>(),
            ModelDb.Card<DistortionScholar>(),
            ModelDb.Card<TelecomEngineeringMaster>(),
            ModelDb.Card<NtrGodfather>(),
            ModelDb.Card<HalfFruit>(),
            ModelDb.Card<ChuunibyouNote>(),
            ModelDb.Card<CuteNe>(),
            ModelDb.Card<AnimeMaster>(),
            ModelDb.Card<MadIsDeadEnd>(),
            ModelDb.Card<Vector>(),
            ModelDb.Card<BrainWife>(),
            ModelDb.Card<Transference>(),
            ModelDb.Card<Distort>(),
            ModelDb.Card<RedStone>(),
            ModelDb.Card<BigNose>(),
            ModelDb.Card<RainyProtocol>(),
        ];
    }
}
