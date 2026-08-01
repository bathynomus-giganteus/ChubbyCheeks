using BaseLib.Abstracts;
using CultLeaderMod.CultLeaderModCode.Extensions;
using Godot;

namespace CultLeaderMod.CultLeaderModCode.Character;

public class CultLeaderModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => CultLeaderMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}