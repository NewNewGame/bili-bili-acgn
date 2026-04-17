//****************** 代码文件申明 ***********************
//* 文件：CardBaseModel
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:00 星期四
//* 描述：卡牌基类模型，提供卡牌图片路径等通用能力
//*******************************************************
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

public abstract class CardBaseModel : CustomCardModel
{
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "none.png".CardImagePath();
        }
    }
    public CardBaseModel(int canonicalEnergyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true) : base(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}