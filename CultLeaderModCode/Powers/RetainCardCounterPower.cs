using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden combat tracker counting cards retained during end-of-turn flushes.
/// </summary>
[RegisterPower]
public class RetainCardCounterPower : ModPowerTemplate
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
        if (owner == null || owner.GetPower<RetainCardCounterPower>() != null)
            return;

        await PowerCmd.Apply<RetainCardCounterPower>(
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
        return owner?.GetPower<RetainCardCounterPower>()?.Total ?? 0;
    }

    public override async Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        await base.AfterFlush(choiceContext, player, flushedCards, retainedCards);

        if (player.Creature != base.Owner || retainedCards == null)
            return;

        GetInternalData<Data>().Total += retainedCards.Count;
    }
}
