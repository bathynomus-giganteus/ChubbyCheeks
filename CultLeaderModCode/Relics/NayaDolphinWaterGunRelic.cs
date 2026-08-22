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
public class NayaDolphinWaterGunRelic : CultLeaderModRelic
{
    private int _regenGains;

    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/naya_dolphin_water_gun.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => _regenGains % 8;

    [SavedProperty]
    public int RegenGains
    {
        get => _regenGains;
        set
        {
            AssertMutable();
            _regenGains = value;
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
        if (amount <= 0m || power.Owner != base.Owner.Creature)
            return;
        if (power is not HealingPower and not LifeEssencePower)
            return;

        RegenGains++;
        if (RegenGains % 8 == 0)
        {
            Flash();
            await CreatureCmd.Heal(base.Owner.Creature, 1m);
            await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        }
    }
}
