using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class MagicBatteryRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/magic_battery.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/magic_battery.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/magic_battery.png";
}
