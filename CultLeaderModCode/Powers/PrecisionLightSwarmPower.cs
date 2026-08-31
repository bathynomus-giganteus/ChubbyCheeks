using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Precision Light Swarm. Whenever the player applies a debuff to an enemy,
/// that enemy takes damage equal to twice the applied stack count.
/// </summary>
[RegisterPower]
public class PrecisionLightSwarmPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_欧若拉.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_欧若拉.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount <= 0m || base.Owner == null)
            return;
        if (power.Type != PowerType.Debuff || power.Owner == null || !power.Owner.IsMonster)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            power.Owner,
            amount * 2m,
            ValueProp.Unpowered,
            base.Owner,
            null,
            null
        );
    }
}
