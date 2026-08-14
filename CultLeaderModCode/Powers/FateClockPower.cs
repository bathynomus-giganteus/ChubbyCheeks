using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Fate Clock. At the start of this enemy's next turn, it takes damage equal to its stacks, then the power is removed.
/// </summary>
[RegisterPower]
public class FateClockPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/clock.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/clock.png";

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);

        if (side != CombatSide.Enemy || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        Flash();
        await CreatureCmd.Damage(
            choiceContext,
            base.Owner,
            base.Amount,
            ValueProp.Unpowered,
            base.Applier ?? base.Owner
        );
        await PowerCmd.Remove(this);
    }
}