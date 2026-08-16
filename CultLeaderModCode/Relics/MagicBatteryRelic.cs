using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class MagicBatteryRelic : CultLeaderModRelic
{
    private int _energyAccumulator;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/magic_battery.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/magic_battery.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/magic_battery.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => EnergyAccumulator;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new EnergyVar(20)];

    [SavedProperty]
    public int EnergyAccumulator
    {
        get => _energyAccumulator;
        set
        {
            AssertMutable();
            _energyAccumulator = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner != base.Owner || amount <= 0)
            return;

        EnergyAccumulator += amount;
        while (EnergyAccumulator >= 20)
        {
            EnergyAccumulator -= 20;

            var enemies = ApostleCardEffectHelpers.AliveEnemies(base.Owner.Creature);
            if (enemies.Count == 0)
                continue;

            Flash();
            foreach (var enemy in enemies)
            {
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    enemy,
                    10m,
                    ValueProp.Unpowered,
                    base.Owner.Creature,
                    null,
                    null
                );
            }
        }
    }
}
