using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
[RegisterCharacterStarterCard(typeof(CultLeaderModCharacter), 1)]
public class TestAddApostleCards : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/test_add_cards.png");

    public TestAddApostleCards()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = base.Owner;
        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            Entry.Logger.Warn("[TEST] No combat state; cannot offer random recruits.");
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
        await CardPileCmd.Add(chosen, PileType.Hand, CardPilePosition.Top, this, false);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}