using CultLeaderMod.CultLeaderModCode.Cards;
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
/// Hidden combat tracker counting every debuff stack applied to enemies over the whole combat.
/// </summary>
[RegisterPower]
public class DebuffAppliedTrackerPower : ModPowerTemplate
{
    private sealed class Data
    {
        public int Total;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public int Total => GetInternalData<Data>().Total;

    public static async Task EnsureTracker(
        PlayerChoiceContext choiceContext,
        Creature owner,
        Creature? applier,
        CardModel? cardSource)
    {
        if (owner == null || owner.GetPower<DebuffAppliedTrackerPower>() != null)
            return;

        await PowerCmd.Apply<DebuffAppliedTrackerPower>(
            choiceContext,
            owner,
            1m,
            applier ?? owner,
            cardSource,
            silent: true
        );
    }

    public static int GetTotal(Creature owner)
    {
        return owner?.GetPower<DebuffAppliedTrackerPower>()?.Total ?? 0;
    }

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

        GetInternalData<Data>().Total += 1;

        var player = base.Owner?.Player;
        if (player != null)
            Apostle_Melancholy_13.RecordDebuffApplied(player);
    }
}
