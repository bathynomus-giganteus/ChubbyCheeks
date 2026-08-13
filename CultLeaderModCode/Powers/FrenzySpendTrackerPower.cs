using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden combat tracker that records how much Vigor/Fervor the player has spent.
/// Used by Frenzy cards that count consumed stacks (both this turn and over the whole combat).
/// </summary>
[RegisterPower]
public class FrenzySpendTrackerPower : ModPowerTemplate
{
    private sealed class Data
    {
        public int TotalConsumed;
        public int TurnConsumed;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public int TotalConsumed => GetInternalData<Data>().TotalConsumed;
    public int TurnConsumed => GetInternalData<Data>().TurnConsumed;

    public static async Task EnsureTracker(PlayerChoiceContext choiceContext, Creature owner, Creature? applier, CardModel? cardSource)
    {
        if (owner == null || owner.Powers?.OfType<FrenzySpendTrackerPower>().Any() == true)
            return;

        await PowerCmd.Apply<FrenzySpendTrackerPower>(
            choiceContext,
            owner,
            1m,
            applier ?? owner,
            cardSource,
            silent: true
        );
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount >= 0m || Owner == null)
            return;

        if ((power is VigorPower || power is FervorPower) && power.Owner == Owner)
        {
            var data = GetInternalData<Data>();
            data.TotalConsumed += (int)(-amount);
            data.TurnConsumed += (int)(-amount);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature == Owner)
            GetInternalData<Data>().TurnConsumed = 0;
    }
}