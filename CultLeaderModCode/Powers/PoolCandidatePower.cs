using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Pool Candidate. At the end of the player's turn, gain Plating stacks.
/// At the start of the player's turn, trigger Plating once by granting Block equal to current Plating stacks.
/// </summary>
[RegisterPower]
public class PoolCandidatePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_09.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_09.png";

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side != CombatSide.Player || base.Owner == null || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        await ApostleCardPlayHelpers.ApplyCalmPower(
            choiceContext,
            base.Owner,
            base.Amount,
            base.Owner,
            null
        );
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Owner == null)
            return;

        decimal plating = (base.Owner.GetPower<PlatingPower>()?.Amount ?? 0m)
                        + (base.Owner.GetPower<SolidIcePower>()?.Amount ?? 0m);
        if (plating <= 0m)
            return;

        await CreatureCmd.GainBlock(base.Owner, plating, ValueProp.Unpowered, null);
    }
}
