using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class HappinessOfYongchunRelic : CultLeaderModRelic
{
    [SavedProperty]
    public int PersonalityMask { get; set; }

    [SavedProperty]
    public int FateChoice { get; set; }

    [SavedProperty]
    public int HighRarityBonusPercent { get; set; }

    [SavedProperty]
    public int ChaosDescriptionVariant { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/happiness_of_Yongchun.png";

    public override async Task AfterObtained()
    {
        if (PersonalityMask == 0 && GumBlessRelic.GetSelectedTags(Owner) is { Count: 2 } selected)
            PersonalityMask = GumBlessRelic.EncodeSelection(selected);
        await OfferRareCardRewards();
    }

    public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        if (player != Owner || !GumBlessRelic.HasSelection(player))
            return options;

        var existingFilter = options.CardPoolFilter;
        return options.WithFilter(card =>
        {
            if (existingFilter != null && !existingFilter(card))
                return false;
            if (GumBlessRelic.IsUnselectedPersonalityCard(card, player))
                return player.PlayerRng.Rewards.NextDouble() >= 0.85;
            return true;
        });
    }

    public override CardRarity ModifyMerchantCardRarity(Player player, CardRarity rarity) =>
        player == Owner ? GumBlessRelic.ApplyMerchantRarityBonus(player, rarity) : rarity;

    private async Task OfferRareCardRewards()
    {
        var candidates = ModelDb.AllCards
            .Where(card => ApostlePowerRules.IsApostleCard(card) &&
                           card.Rarity == CardRarity.Rare &&
                           card.CanBeGeneratedInCombat)
            .OrderBy(card => card.Id.ToString(), StringComparer.Ordinal)
            .OrderBy(_ => Owner.PlayerRng.Rewards.NextInt())
            .Take(15)
            .Select(card => Owner.RunState.CreateCard(card, Owner))
            .ToList();
        if (candidates.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, 2)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(), candidates, Owner, prefs)).ToList();
        if (selected.Count == 0)
            return;

        var addResults = await CardPileCmd.Add(selected, PileType.Deck);
        CardCmd.PreviewCardPileAdd(addResults, 2f);
    }

}
