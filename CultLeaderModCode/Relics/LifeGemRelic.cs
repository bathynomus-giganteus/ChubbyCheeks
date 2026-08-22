using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class LifeGemRelic : CultLeaderModRelic
{
    private int _roomsEntered;

    public override RelicRarity Rarity => RelicRarity.Common;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/life_gem.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/life_gem.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/life_gem.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _roomsEntered;

    [SavedProperty]
    public int RoomsEntered
    {
        get => _roomsEntered;
        set
        {
            AssertMutable();
            _roomsEntered = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        RoomsEntered++;
        if (RoomsEntered < 2)
            return;

        RoomsEntered = 0;
        Flash();
        await CreatureCmd.GainMaxHp(base.Owner.Creature, 1m);
    }
}
