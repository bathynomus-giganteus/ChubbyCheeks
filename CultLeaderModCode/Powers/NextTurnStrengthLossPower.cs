using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class NextTurnStrengthLossPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_18.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_18.png";

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        if (side != CombatSide.Enemy || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        await PowerCmd.Apply<TempStrengthLossPower>(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            base.Amount,
            base.Applier ?? base.Owner,
            null
        );
        await PowerCmd.Remove(this);
    }
}
