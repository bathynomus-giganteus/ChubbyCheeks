using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class MeditationTimeRelic : CultLeaderModRelic
{
    private int _restSiteChoicesTaken;

    public override RelicRarity Rarity => RelicRarity.Shop;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/meditation_time.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/meditation_time.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/meditation_time.png";

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType == RoomType.RestSite)
            _restSiteChoicesTaken = 0;

        return Task.CompletedTask;
    }

    public override bool ShouldDisableRemainingRestSiteOptions(Player player)
    {
        if (player != base.Owner)
            return true;

        Flash();
        _restSiteChoicesTaken++;
        return _restSiteChoicesTaken >= 2;
    }
}