using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class SeidouCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Cards", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/seidou.png");

    public SeidouCard()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scope = base.CardScope;
        if (scope == null)
            return;

        var apostleCards = ModelDb.AllCards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle))
            .Where(card => card.CanBeGeneratedInCombat)
            .OrderBy(_ => Random.Shared.Next())
            .Take(DynamicVars["Cards"].IntValue)
            .ToList();

        if (apostleCards.Count == 0)
            return;

        var created = apostleCards
            .Select(card => scope.CreateCard(card, base.Owner))
            .ToList();

        await CardPileCmd.Add(created, PileType.Draw, CardPilePosition.Random, this, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(2m);
    }
}
