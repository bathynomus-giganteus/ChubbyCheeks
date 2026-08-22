using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Powers;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class ErfenStaffRelic : CultLeaderModRelic
{
    private int _powerGains;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/erfen_staff.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/erfen_staff.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/erfen_staff.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _powerGains % 10;

    [SavedProperty]
    public int PowerGains
    {
        get => _powerGains;
        set
        {
            AssertMutable();
            _powerGains = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource
    )
    {
        if (amount <= 0m || power.Owner != base.Owner.Creature || !IsTrackedPower(power))
            return;

        PowerGains++;
        if (PowerGains % 10 == 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(1m, base.Owner);
        }
    }

    private static bool IsTrackedPower(PowerModel power)
    {
        return power is HealingPower
            or LifeEssencePower
            or VigorPower
            or FervorPower
            or PlatingPower
            or SolidIcePower
            or RetainPower
            or HappinessPower
            or BitterPainPower
            or BitterPainBurstPower;
    }
}
