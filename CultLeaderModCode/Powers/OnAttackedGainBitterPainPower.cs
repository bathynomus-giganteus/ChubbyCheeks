using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Elen-A Overclock. Whenever an enemy attack connects with the owner, gain Bitter Pain stacks.
/// </summary>
[RegisterPower]
public class OnAttackedGainBitterPainPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_18.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_18.png";

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);

        if (base.Amount <= 0 || command.Attacker == null || !command.Attacker.IsMonster)
            return;

        bool hitOwner = command.Results
            .SelectMany(hits => hits)
            .Any(result => result.Receiver == base.Owner);

        if (!hitOwner)
            return;

        await ApostlePowerRules.ApplyApostlePower<BitterPainPower, BitterPainBurstPower>(
            choiceContext,
            base.Owner,
            base.Amount,
            command.Attacker,
            null
        );
    }
}