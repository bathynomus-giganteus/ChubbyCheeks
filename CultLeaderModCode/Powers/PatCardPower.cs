using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden tracker for the "摸摸头" card. Marks one or more apostle cards; each
/// time one of those cards is played this combat, draw one card.
/// </summary>
[RegisterPower]
public class PatCardPower : ModPowerTemplate
{
    private sealed class Data
    {
        public List<CardModel> MarkedCards = new();
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void MarkCard(CardModel card)
    {
        if (card == null)
            return;

        var data = GetInternalData<Data>();
        if (!data.MarkedCards.Contains(card))
            data.MarkedCards.Add(card);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        var data = GetInternalData<Data>();
        if (data.MarkedCards.Count == 0)
            return;

        if (!cardPlay.IsLastInSeries)
            return;

        var playedCard = cardPlay.Card;
        if (playedCard == null || !data.MarkedCards.Contains(playedCard))
            return;

        var player = base.Owner.Player;
        if (player == null)
            return;

        await CardPileCmd.Draw(choiceContext, 1m, player);
    }
}
