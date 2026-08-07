using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
[RegisterCharacterStarterRelic(typeof(CultLeaderModCharacter))]
public class GumBlessRelic : CultLeaderModRelic
{
    private static readonly CardTag[] PersonalityTags =
    [
        CultLeaderCardTags.Pure,
        CultLeaderCardTags.Calm,
        CultLeaderCardTags.Frenzy,
        CultLeaderCardTags.Lively,
        CultLeaderCardTags.Melancholy
    ];

    private static readonly Random _rng = new();

    public static HashSet<CardTag>? SelectedTags { get; internal set; }
    public static HashSet<CardTag>? UnselectedTags { get; internal set; }
    public static bool SelectionMade => SelectedTags != null;
    public static bool SelectionInProgress { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/gum_bless.png";
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/gum_bless.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/gum_bless.png";

    public override Task AfterObtained()
    {
        ResetSelection();
        Entry.Logger.Info("[GumBlessRelic] Opening personality selection queued until Neow event UI is ready.");
        return Task.CompletedTask;
    }

    public static void ResetSelection()
    {
        SelectedTags = null;
        UnselectedTags = null;
        SelectionInProgress = false;
    }

    public static void SetSelection(HashSet<CardTag> selected, HashSet<CardTag> unselected)
    {
        SelectedTags = selected;
        UnselectedTags = unselected;
        SelectionInProgress = false;
        UpdateRelicDescription();
    }

    public static bool IsUnselectedPersonalityCard(CardModel card)
    {
        if (!SelectionMade || UnselectedTags == null) return false;
        var tags = card.Tags;

        if (tags.Contains(CultLeaderCardTags.Pure) &&
            tags.Contains(CultLeaderCardTags.Calm) &&
            tags.Contains(CultLeaderCardTags.Frenzy) &&
            tags.Contains(CultLeaderCardTags.Lively) &&
            tags.Contains(CultLeaderCardTags.Melancholy))
            return false;

        foreach (var tag in UnselectedTags)
        {
            if (tags.Contains(tag)) return true;
        }
        return false;
    }

    public static bool ShouldOfferOpeningSelection(Player player)
    {
        return player.Character is CultLeaderModCharacter && !SelectionMade && !SelectionInProgress;
    }

    public static async Task<bool> TriggerOpeningSelection(Player player)
    {
        if (SelectionMade || SelectionInProgress)
        {
            return SelectionMade;
        }

        if (player.Character is not CultLeaderModCharacter)
        {
            return false;
        }

        SelectionInProgress = true;

        try
        {
            if (!LocalContext.IsMe(player))
            {
                Entry.Logger.Warn($"[GumBlessRelic] LocalContext was not bound to starter relic owner. Binding NetId={player.NetId} before opening selection.");
                LocalContext.NetId = player.NetId;
            }

            var runState = player.RunState;
            var cards = new List<CardModel>
            {
                runState.CreateCard<PersonalitySelectPureCard>(player),
                runState.CreateCard<PersonalitySelectCalmCard>(player),
                runState.CreateCard<PersonalitySelectFrenzyCard>(player),
                runState.CreateCard<PersonalitySelectLivelyCard>(player),
                runState.CreateCard<PersonalitySelectMelancholyCard>(player),
            };

            var prefs = new CardSelectorPrefs(new LocString("gameplay_ui", "CULT_LEADER_PERSONALITY_SELECTION.prompt"), 2)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };

            Entry.Logger.Info("[GumBlessRelic] Showing opening 5-pick-2 personality selection.");
            var selectedCards = (await CardSelectCmd.FromSimpleGrid(
                new BlockingPlayerChoiceContext(),
                cards,
                player,
                prefs)).ToList();

            if (selectedCards.Count != 2)
            {
                Entry.Logger.Warn($"[GumBlessRelic] Expected 2 selected cards, got {selectedCards.Count}.");
            }

            var selectedTags = selectedCards
                .Select(GetPersonalityTag)
                .Where(tag => tag.HasValue)
                .Select(tag => tag!.Value)
                .ToHashSet();

            SetSelection(selectedTags, PersonalityTags.Except(selectedTags).ToHashSet());
            Entry.Logger.Info($"[GumBlessRelic] Opening selection complete: {string.Join(", ", selectedTags)}");
            return true;
        }
        catch (Exception ex)
        {
            SelectionInProgress = false;
            Entry.Logger.Error($"[GumBlessRelic] Opening selection failed: {ex}");
            return false;
        }
    }

    private static CardTag? GetPersonalityTag(CardModel card)
    {
        var typeName = card.GetType().Name;
        if (typeName == nameof(PersonalitySelectPureCard)) return CultLeaderCardTags.Pure;
        if (typeName == nameof(PersonalitySelectCalmCard)) return CultLeaderCardTags.Calm;
        if (typeName == nameof(PersonalitySelectFrenzyCard)) return CultLeaderCardTags.Frenzy;
        if (typeName == nameof(PersonalitySelectLivelyCard)) return CultLeaderCardTags.Lively;
        if (typeName == nameof(PersonalitySelectMelancholyCard)) return CultLeaderCardTags.Melancholy;
        return null;
    }

    private static string GetPersonalityName(CardTag tag)
    {
        if (tag == CultLeaderCardTags.Pure) return "纯粹";
        if (tag == CultLeaderCardTags.Calm) return "冷静";
        if (tag == CultLeaderCardTags.Frenzy) return "狂热";
        if (tag == CultLeaderCardTags.Lively) return "活泼";
        if (tag == CultLeaderCardTags.Melancholy) return "忧郁";
        return "???";
    }

    private static void UpdateRelicDescription()
    {
        if (SelectedTags == null || SelectedTags.Count != 2) return;

        var names = SelectedTags.Select(GetPersonalityName).ToList();
        var description = $"{names[0]}和{names[1]}使徒的出现概率提升。";

        try
        {
            var relicsTable = LocManager.Instance.GetTable("relics");
            relicsTable.MergeWith(new System.Collections.Generic.Dictionary<string, string>
            {
                ["CULT_LEADER_MOD_RELIC_GUM_BLESS_RELIC.description"] = description
            });
            Entry.Logger.Info($"[GumBlessRelic] Updated relic description: {description}");
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[GumBlessRelic] Failed to update relic description: {ex}");
        }
    }

    
    /// <summary>
    /// Filter a list of cards, removing unselected personality cards (85% rejection rate).
    /// Returns a new list; if no cards were filtered, returns the original list.
    /// </summary>
    public static List<CardModel> FilterUnselectedCards(List<CardModel> cards)
    {
        if (!SelectionMade || UnselectedTags == null) return cards;

        var filtered = new List<CardModel>(cards.Count);
        bool changed = false;

        foreach (var card in cards)
        {
            if (IsUnselectedPersonalityCard(card))
            {
                // 85% chance to reject unselected personality cards
                if (_rng.NextDouble() >= 0.85)
                {
                    filtered.Add(card);
                }
                else
                {
                    changed = true;
                }
            }
            else
            {
                filtered.Add(card);
            }
        }

        return changed ? filtered : cards;
    }
public override CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)
    {
        try
        {
            if (!SelectionMade)
            {
                Entry.Logger.Info("[GumBlessRelic] ModifyCardRewardCreationOptions: selection not made, returning original");
                return options;
            }

            var existingFilter = options.CardPoolFilter;
            Entry.Logger.Info($"[GumBlessRelic] ModifyCardRewardCreationOptions: applying filter, UnselectedTags={UnselectedTags?.Count ?? 0}");

            return options.WithFilter(card =>
            {
                try
                {
                    if (existingFilter != null && !existingFilter(card)) return false;

                    if (IsUnselectedPersonalityCard(card))
                    {
                        return _rng.NextDouble() >= 0.85;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Entry.Logger.Error($"[GumBlessRelic] Filter error for card {card?.GetType().Name}: {ex}");
                    return true; // keep card on error
                }
            });
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[GumBlessRelic] ModifyCardRewardCreationOptions error: {ex}");
            return options;
        }
    }
}


