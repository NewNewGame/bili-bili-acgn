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
        // 所有[Pool(typeof(FunShikiCardPool))]的卡牌
        return [
            ModelDb.Card<QianDaJiang>(),
            ModelDb.Card<MawaruMawaru>(),
            ModelDb.Card<ChampionPowder>(),
            ModelDb.Card<FunTuan>(),
            ModelDb.Card<Penguin>(),
            ModelDb.Card<Dress>(),
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
            ModelDb.Card<SuKi>(),
            ModelDb.Card<RedStone>(),
            ModelDb.Card<BigNose>(),
            ModelDb.Card<RainyProtocol>(),
            ModelDb.Card<FunShikiEcstasy>(),
            ModelDb.Card<FoodTaster>(),
            ModelDb.Card<ConsecutiveBaiDa>(),
            ModelDb.Card<OneColorOrb>(),
            ModelDb.Card<Birthday>(),
            ModelDb.Card<MarriedMan>(),
            ModelDb.Card<KneelToBaiDa>(),
            ModelDb.Card<LuckyHit>(),
            ModelDb.Card<ManManEel>(),
            ModelDb.Card<Dcm>(),
            ModelDb.Card<WanCeJin>(),
            ModelDb.Card<BambooFishDinner>(),
            ModelDb.Card<GravityCircle>(),
            ModelDb.Card<EiHeiJiang>(),
            ModelDb.Card<MisunderstandingMad>(),
            ModelDb.Card<SheIsDifferent>(),
            ModelDb.Card<Pantyhose>(),
            ModelDb.Card<Archbishop>(),
            ModelDb.Card<NobleLady>(),
            ModelDb.Card<BlackRabbit>(),
            ModelDb.Card<NewSeasonTimeMachine>(),
            ModelDb.Card<UnreachableFlower>(),
            ModelDb.Card<FunShikiOnmyoji>(),
            ModelDb.Card<FunPhilosophy>(),
            ModelDb.Card<SelfHistoryLeak>(),
            ModelDb.Card<NewSeasonWonderHouse>(),
            ModelDb.Card<AloftThrone>(),
            ModelDb.Card<LoveBrain>(),
            ModelDb.Card<AhShortHaircut>(),
            ModelDb.Card<SailorUniform>(),
            ModelDb.Card<DestinyStage>(),
            ModelDb.Card<TasteHistorian>(),
            ModelDb.Card<NewSeasonGuide>(),
            ModelDb.Card<ThreePointEight>(),
            ModelDb.Card<AnimeEmperor>(),
            ModelDb.Card<BigVillaMansion>(),
            ModelDb.Card<CrazyStackArmor>(),
            ModelDb.Card<UtterMadness>(),
            ModelDb.Card<TataKai>(),
            ModelDb.Card<FanBaYe>(),
            ModelDb.Card<FanStyleDiva>(),
            ModelDb.Card<UncleStrike>(),
            ModelDb.Card<WhirlwindStrike>(),
            ModelDb.Card<Boomerang>(),
            ModelDb.Card<FunShikiDefend>(),
            ModelDb.Card<ZeroFruit>(),
            ModelDb.Card<MangaArtistKirito>(),
            ModelDb.Card<MinusTwentyMillionFruit>(),
            ModelDb.Card<Aishiteru>(),
            ModelDb.Card<DoZatan>(),
            ModelDb.Card<BuyVideo>(),
            ModelDb.Card<BoostVideo>(),
            ModelDb.Card<FunCaptain>(),
            ModelDb.Card<FinaleRoast>(),
            ModelDb.Card<KuyaXi>(),
            ModelDb.Card<GreenTeaFan>(),
            ModelDb.Card<WhirlwindBlock>(),
            ModelDb.Card<MilkBurstNewSeason>(),
            ModelDb.Card<AboutToChunFall>(),
            ModelDb.Card<FanRemixAtelier>(),
            ModelDb.Card<FrenchAccent>(),
            ModelDb.Card<SuddenUpdate>(),
        ];
    }
}
