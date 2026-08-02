using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public sealed class RandomRecruitment : CultLeaderModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Exhaust];
    public override string CustomPortraitPath => "random_recruitment.png".BigCardImagePath();
    public override string PortraitPath => "random_recruitment.png".CardImagePath();
    public override string BetaPortraitPath => PortraitPath;

    public RandomRecruitment() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        // Get all Pure Apostle cards from the card pool
        var allPureCards = Owner.Character.CardPool.AllCards
            .Where(c => c is IApostleCard a && a.Personality == ApostlePersonality.Pure)
            .ToList();

        foreach (var card in allPureCards)
        {
            var instance = combatState.CreateCard(card, Owner);
            await CardPileCmd.Add(instance, (PileType)1, (CardPilePosition)1, null, false);
        }

        // Draw to fill hand
        await CardPileCmd.Draw(choiceContext, 10m, Owner, false);
    }
}