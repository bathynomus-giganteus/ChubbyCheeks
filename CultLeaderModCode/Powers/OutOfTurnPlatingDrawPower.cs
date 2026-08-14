using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Tactical Satellite. Whenever the owner gains Plating outside their own turn, draw a card.
/// </summary>
[RegisterPower]
public class OutOfTurnPlatingDrawPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_04.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_04.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount <= 0m || base.Owner == null || power.Owner != base.Owner)
            return;
        if (power is not PlatingPower and not SolidIcePower)
            return;

        var combatState = base.Owner.CombatState;
        if (combatState == null || combatState.CurrentSide == base.Owner.Side)
            return;

        var player = base.Owner.Player;
        if (player == null)
            return;

        await CardPileCmd.Draw(choiceContext, 1m, player);
    }
}