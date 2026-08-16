using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class ShediDimensionScytheRelic : CultLeaderModRelic
{
    private const int RequiredCombats = 3;

    private int _nonBossCombats;
    private bool _freeTravelReady;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/shedi_dimension_scythe.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/shedi_dimension_scythe.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/shedi_dimension_scythe.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _freeTravelReady ? RequiredCombats : _nonBossCombats;

    [SavedProperty]
    public int NonBossCombats
    {
        get => _nonBossCombats;
        set
        {
            AssertMutable();
            _nonBossCombats = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public bool FreeTravelReady
    {
        get => _freeTravelReady;
        set
        {
            AssertMutable();
            _freeTravelReady = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShouldAllowFreeTravel()
    {
        return FreeTravelReady;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType == RoomType.Boss)
            return Task.CompletedTask;

        NonBossCombats++;
        if (NonBossCombats >= RequiredCombats && !FreeTravelReady)
            FreeTravelReady = true;

        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!FreeTravelReady || base.Owner.RunState is not RunState runState || runState.VisitedMapCoords.Count <= 1)
            return Task.CompletedTask;

        var visitedMapCoords = runState.VisitedMapCoords;
        var previousCoord = visitedMapCoords[visitedMapCoords.Count - 2];
        var previousPoint = runState.Map.GetPoint(previousCoord);
        var currentPoint = runState.CurrentMapPoint;
        if (previousPoint == null || currentPoint == null || previousPoint.Children.Contains(currentPoint))
            return Task.CompletedTask;

        FreeTravelReady = false;
        NonBossCombats = 0;
        return Task.CompletedTask;
    }
}
