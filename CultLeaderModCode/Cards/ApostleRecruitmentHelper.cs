using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Cards;

internal static class ApostleRecruitmentHelper
{
    public static async Task OfferRandomApostleToHandForFreeThisTurn(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel source)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            Entry.Logger.Warn("[ApostleRecruitment] No combat state; cannot offer random recruits.");
            return;
        }

        var allApostleCards = ModelDb.AllCards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle))
            .Where(card => card.CanBeGeneratedInCombat)
            .ToList();

        // Honor the starting relic's personality weighting before sampling.
        var weightedPool = GumBlessRelic.FilterUnselectedCards(allApostleCards);

        var apostleCards = weightedPool
            .OrderBy(_ => Random.Shared.Next())
            .Take(3)
            .ToList();

        if (apostleCards.Count == 0)
            return;

        var options = apostleCards
            .Select(card => combatState.CreateCard(card, player))
            .ToList();

        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, player);
        if (chosen == null)
            return;

        chosen.EnergyCost.SetThisTurn(0);
        await CardPileCmd.Add(chosen, PileType.Hand, CardPilePosition.Top, source, false);
    }
}
