using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class NayaDolphinWaterGunRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";
}
