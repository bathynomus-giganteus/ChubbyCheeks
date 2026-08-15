using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class ButterYellowCardRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/butter_yellow_card.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/butter_yellow_card.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/butter_yellow_card.png";
}
