using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden tracker for ???Time. Counts other cards played this turn so the
/// corresponding card can disable itself until the end of the turn.
/// </summary>
[RegisterPower]
public class SoftTimePlayTrackerPower : ModPowerTemplate
{
    private sealed class Data
    {
        public int OtherCardsPlayed;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public int OtherCardsPlayed => GetInternalData<Data>().OtherCardsPlayed;

    public static async Task EnsureTracker(
        PlayerChoiceContext choiceContext,
        Creature owner,
        Creature? applier,
        CardModel? cardSource)
    {
        if (owner == null || owner.GetPower<SoftTimePlayTrackerPower>() != null)
            return;

        await PowerCmd.Apply<SoftTimePlayTrackerPower>(
            choiceContext,
            owner,
            1m,
            applier ?? owner,
            cardSource,
            silent: true
        );
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        if (!cardPlay.IsLastInSeries)
            return;
        if (cardPlay.Card is Apostle_Melancholy_01)
            return;

        GetInternalData<Data>().OtherCardsPlayed++;

        if (base.Owner.Player is { } player)
        {
            foreach (var card in PileType.Hand.GetPile(player).Cards)
            {
                NCard.FindOnTable(card)?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            }
        }
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner)
            return;

        GetInternalData<Data>().OtherCardsPlayed = 0;
    }
}
