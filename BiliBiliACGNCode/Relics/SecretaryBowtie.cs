//****************** 代码文件申明 ***********************
//* 文件：SecretaryBowtie(书记领结)
//* 作者：wheat
//* 创建时间：2026/04/27 17:30:20 星期一
//* 描述：你每洗牌3次，获得2点能量。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SecretaryBowtie : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Turns", 3m)];
    private int _shuffleCount = 0;
    [SavedProperty]
    public int BILIBILIACGN_SB_ShuffleCount
    {
        get
        {
            return _shuffleCount;
        }
        private set
        {
            AssertMutable();
            _shuffleCount = value;
            InvokeDisplayAmountChanged();
        }
    }
    public override int DisplayAmount => BILIBILIACGN_SB_ShuffleCount;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if(shuffler == base.Owner)
        {
            BILIBILIACGN_SB_ShuffleCount++;
            if(BILIBILIACGN_SB_ShuffleCount >= (int)base.DynamicVars["Turns"].BaseValue)
            {
                Flash();
                BILIBILIACGN_SB_ShuffleCount = 0;
                await PlayerCmd.GainEnergy(2, base.Owner);
            }
        }
    }
}

