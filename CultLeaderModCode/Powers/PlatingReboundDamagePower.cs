using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
/// Stinging Gatekeeper. Whenever an enemy attack connects with the owner, deal damage to that enemy
/// equal to the owner's current Plating stacks multiplied by this power's stacks.
/// </summary>
[RegisterPower]
public class PlatingReboundDamagePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_06.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_06.png";

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);

        if (command.Attacker == null || !command.Attacker.IsMonster)
            return;

        bool hitOwner = command.Results
            .SelectMany(hits => hits)
            .Any(result => result.Receiver == base.Owner);

        if (!hitOwner)
            return;

        decimal plating = (base.Owner?.GetPower<PlatingPower>()?.Amount ?? 0m)
                        + (base.Owner?.GetPower<SolidIcePower>()?.Amount ?? 0m);
        if (plating <= 0m)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            command.Attacker,
            plating * base.Amount,
            ValueProp.Unpowered,
            base.Owner!
        );
    }
}