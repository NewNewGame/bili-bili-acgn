//****************** 代码文件申明 ***********************
//* 文件：FanshikiCharacter
//* 作者：wheat
//* 创建时间：2026/04/07
//* 描述：泛式角色
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Potions.PotionPool;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

/*
public sealed class FanshikiCharacter : PlaceholderCharacterModel
{
    public override string PlaceholderID => "fanshiki";
    public override Color NameColor => new Color("E02D16");

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 80;

    public override CardPoolModel CardPool => ModelDb.CardPool<FanshikiCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<FanshikiRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<FanshikiPotionPool>();
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://BiliBiliACGN/images/characters/fanshiki_icon.png";
    protected override string MapMarkerPath => "res://BiliBiliACGN/images/characters/fanshiki_map_marker.png";
    //public override string CustomArmPointingTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_point.png";
    //public override string CustomArmPaperTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_paper.png";
    //public override string CustomArmRockTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_rock.png";
    //public override string CustomArmScissorsTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_scissors.png";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://BiliBiliACGN/images/characters/fanshiki_select_icon.png";
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://BiliBiliACGN/images/characters/fanshiki_select_locked_icon.png";
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string CustomCharacterSelectTransitionPath => "res://BiliBiliACGN/materials/transitions/" + PlaceholderID + "_transition_mat.tres";

    public override IEnumerable<CardModel> StartingDeck => [

    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<EiHeiJiangMask>(),
    ];
}   
*/