//****************** 代码文件申明 ***********************
//* 文件：AttackOrb
//* 作者：wheat
//* 创建时间：2026/04/08
//* 描述：进击充能球
//*******************************************************

using Godot;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Orbs;

public sealed class AttackOrb : OrbBaseModel
{
    protected override string PassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";

	protected override string EvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";

	protected override string ChannelSfx => "event:/sfx/characters/defect/defect_lightning_channel";
    public override decimal PassiveVal => throw new NotImplementedException();

    public override decimal EvokeVal => throw new NotImplementedException();

    public override Color DarkenedColor => throw new NotImplementedException();
}   