using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class StatusUpCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("MaxCards", 1m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/status_up.png");

    public StatusUpCard()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(base.Owner).Cards
            .Where(card => ApostlePowerRules.IsApostleCard(card) && card != this)
            .ToList();

        if (hand.Count == 0)
            return;

        var maxCards = IsUpgraded
            ? DynamicVars["MaxCards"].IntValue
            : 1;
        maxCards = Math.Min(maxCards, hand.Count);

        var prefs = IsUpgraded
            ? new CardSelectorPrefs(base.SelectionScreenPrompt, 0, maxCards)
            : new CardSelectorPrefs(base.SelectionScreenPrompt, 1);

        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            base.Owner,
            prefs,
            card => ApostlePowerRules.IsApostleCard(card) && card != this,
            this);

        foreach (var card in selected)
        {
            var replacement = CreateHigherRarityApostle(card);
            if (replacement == null)
                continue;

            await CardCmd.Transform(card, replacement, CardPreviewStyle.None);
        }
    }

    private CardModel? CreateHigherRarityApostle(CardModel original)
    {
        var scope = base.CardScope;
        if (scope == null)
            return null;

        var candidates = ModelDb.AllCards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle))
            .Where(card => card.CanBeGeneratedInCombat)
            .Where(card => IsTransformableRarity(card.Rarity) && card.Rarity > original.Rarity)
            .ToList();

        if (candidates.Count == 0)
            return null;

        var candidate = candidates[Random.Shared.Next(candidates.Count)];
        return scope.CreateCard(candidate, base.Owner);
    }

    private static bool IsTransformableRarity(CardRarity rarity)
    {
        return rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxCards"].UpgradeValueBy(1m);
    }
}
