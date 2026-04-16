//****************** 代码文件申明 ***********************
//* 文件：FunShikiCharacter
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

namespace BiliBiliACGN.BiliBiliACGNCode.Characters;
public sealed class FunShikiCharacter : PlaceholderCharacterModel
{
	public override string PlaceholderID => "funshiki";
	public override Color NameColor => new Color("E02D16");
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 60;
	public override int BaseOrbSlotCount => 4;
	public override CardPoolModel CardPool => ModelDb.CardPool<FunShikiCardPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FunShikiRelicPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<FunShikiPotionPool>();
	// 人物头像路径。
	public override string CustomIconTexturePath => "res://BiliBiliACGN/images/characters/funshiki_icon.png";
	protected override string MapMarkerPath => "res://BiliBiliACGN/images/characters/funshiki_map_marker.png";
	//public override string CustomArmPointingTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_point.png";
	//public override string CustomArmPaperTexturePath => "res://BiliBiliACGN/images/hands/fanshiki_paper.png";
	//public override string CustomArmRockTexturePath => "res://BiliBiliACGN/images/hands/funshiki_rock.png";
	//public override string CustomArmScissorsTexturePath => "res://BiliBiliACGN/images/hands/funshiki_scissors.png";
	// 人物选择图标。
	public override string CustomCharacterSelectIconPath => "res://BiliBiliACGN/images/characters/funshiki_select_icon.png";
	// 人物选择图标-锁定状态。
	public override string CustomCharacterSelectLockedIconPath => "res://BiliBiliACGN/images/characters/funshiki_select_locked_icon.png";
	// 过渡音效。这个不能删。
	public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
	public override string CustomCharacterSelectTransitionPath => "res://BiliBiliACGN/materials/transitions/" + PlaceholderID + "_transition_mat.tres";

	public override IEnumerable<CardModel> StartingDeck => [
		ModelDb.Card<WhirlwindStrike>(),
		ModelDb.Card<WhirlwindStrike>(),
		ModelDb.Card<WhirlwindStrike>(),
		ModelDb.Card<WhirlwindStrike>(),
		ModelDb.Card<WhirlwindStrike>(),
		ModelDb.Card<FunShikiDefend>(),
		ModelDb.Card<FunShikiDefend>(),
		ModelDb.Card<FunShikiDefend>(),
		ModelDb.Card<FunShikiDefend>(),
		ModelDb.Card<FunShikiDefend>(),
		ModelDb.Card<ZeroFruit>(),
		ModelDb.Card<MangaArtistKirito>(),
	];

	public override IReadOnlyList<RelicModel> StartingRelics => [
		ModelDb.Relic<EiHeiJiangMask>(),
	];
}   
