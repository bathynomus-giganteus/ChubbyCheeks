using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class VigorPerTurnPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomIconPath => "res://CultLeaderMod/images/powers/fervor.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/powers/big/fervor.png";

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);

        if (side != CombatSide.Player || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        await ApostleCardPlayHelpers.ApplyFrenzyPower(
            choiceContext,
            base.Owner,
            base.Amount,
            base.Applier ?? base.Owner,
            null
        );
        await PowerCmd.Remove(this);
    }
}