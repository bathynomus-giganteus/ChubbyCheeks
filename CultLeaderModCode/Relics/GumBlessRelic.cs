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
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Runtime.CompilerServices;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
[RegisterCharacterStarterRelic(typeof(CultLeaderModCharacter))]
public class GumBlessRelic : CultLeaderModRelic
{
    public const int FateUnchosen = 0;
    public const int FatePredetermined = 1;
    public const int FateRandom = 2;
    public const int FateChaosRarity = 3;
    public const int ChaosHighRarityBonusPercent = 20;

    private const ulong RandomFateMixin = 0x52414E444F4DUL;
    private const ulong ChaosFateMixin = 0x4348414F53UL;

    private static readonly string[] ChaosDescriptions =
    [
        "▓▒░ F4T3://0x13?¿ // %%%_NULL ░▒▓",
        "NUL::7F-2A-??::DΞST!NY//人格=∅",
        "█▓▒ S!GNΛL_L0ST / +20%? / ▒▓█",
        "<UNRΞΛDΛBLΞ> C0MM0N≠RΛRΞ :: ???",
        "0xDEAD//¿¿¿//0xBEEF//FATE_OVERRUN",
        "ERR_PERSONALITY_TABLE::∅∅∅::NO_REF",
        "⟦χΛ0S⟧ 001101?? / RARITY↑ / ####",
        "//VOID//命運?=NaN//▓▓▓//20::0xFF",
        "FATE_PTR→[????] :: REDACTED :: ⊘",
        "░0x4348414F53░ / λ=??? / !SYNC?",
        "{DESTINY_BROKEN}::%$#@!::∞/0",
        "▓ N0_PERS0NΛL!TY ▓ RARE++ ▓ ??? ▓"
    ];

    private static readonly CardTag[] PersonalityTags =
    [
        CultLeaderCardTags.Pure,
        CultLeaderCardTags.Calm,
        CultLeaderCardTags.Frenzy,
        CultLeaderCardTags.Lively,
        CultLeaderCardTags.Melancholy
    ];

    private sealed class SelectionState
    {
        public HashSet<CardTag>? Selected;
        public Task<bool>? Pending;
    }
    private static ConditionalWeakTable<Player, SelectionState> States = new();

    [SavedProperty]
    public int PersonalityMask { get; set; }

    [SavedProperty]
    public int FateChoice { get; set; }

    [SavedProperty]
    public int HighRarityBonusPercent { get; set; }

    [SavedProperty]
    public int ChaosDescriptionVariant { get; set; }

    public static HashSet<CardTag>? GetSelectedTags(Player player)
    {
        var mask = player.Relics.OfType<GumBlessRelic>().FirstOrDefault()?.PersonalityMask ?? 0;
        if (mask == 0)
            mask = player.Relics.OfType<HappinessOfYongchunRelic>().FirstOrDefault()?.PersonalityMask ?? 0;
        if (mask != 0)
            return PersonalityTags.Where((_, i) => (mask & (1 << i)) != 0).ToHashSet();
        return States.GetValue(player, _ => new SelectionState()).Selected;
    }
    public static bool HasSelection(Player player) => GetSelectedTags(player)?.Count == 2;

    public static int GetFateChoice(Player player)
    {
        var starter = player.Relics.OfType<GumBlessRelic>().FirstOrDefault();
        var upgraded = player.Relics.OfType<HappinessOfYongchunRelic>().FirstOrDefault();
        var choice = starter?.FateChoice ?? upgraded?.FateChoice ?? FateUnchosen;
        if (choice != FateUnchosen)
            return choice;

        // Saves created before fate choices existed only persisted the personality mask.
        return HasSelection(player) ? FatePredetermined : FateUnchosen;
    }

    public static bool HasFateChoice(Player player) =>
        GetFateChoice(player) != FateUnchosen || HasSelection(player);

    public static int GetHighRarityBonusPercent(Player player) =>
        player.Relics.OfType<GumBlessRelic>().FirstOrDefault()?.HighRarityBonusPercent
        ?? player.Relics.OfType<HappinessOfYongchunRelic>().FirstOrDefault()?.HighRarityBonusPercent
        ?? 0;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/gum_bless.png";
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/gum_bless.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/gum_bless.png";

    public override Task AfterObtained()
    {
        Entry.Logger.Info("[GumBlessRelic] Opening personality selection queued until Neow event UI is ready.");
        return Task.CompletedTask;
    }

    public static void ResetSelection()
    {
        States = new();
    }


    public static void SetSelection(
        Player player,
        HashSet<CardTag> selected,
        int fateChoice = FatePredetermined)
    {
        States.GetValue(player, _ => new SelectionState()).Selected = selected;
        foreach (var relic in player.Relics.OfType<GumBlessRelic>())
            ApplyFateState(relic, fateChoice, EncodeSelection(selected), 0, 0);
        foreach (var relic in player.Relics.OfType<HappinessOfYongchunRelic>())
            ApplyFateState(relic, fateChoice, EncodeSelection(selected), 0, 0);
    }

    public static void SelectRandomFate(Player player)
    {
        var rng = new Rng(player, ModelDb.Relic<GumBlessRelic>().Id, RandomFateMixin);
        var selected = PersonalityTags
            .OrderBy(_ => rng.NextInt())
            .Take(2)
            .ToHashSet();
        SetSelection(player, selected, FateRandom);
        Entry.Logger.Info($"[GumBlessRelic] Random fate selected: {string.Join(", ", selected)}");
    }

    public static void SelectChaosRarityFate(Player player)
    {
        var rng = new Rng(player, ModelDb.Relic<GumBlessRelic>().Id, ChaosFateMixin);
        var variant = rng.NextInt(ChaosDescriptions.Length);
        States.GetValue(player, _ => new SelectionState()).Selected = null;
        foreach (var relic in player.Relics.OfType<GumBlessRelic>())
            ApplyFateState(relic, FateChaosRarity, 0, ChaosHighRarityBonusPercent, variant);
        foreach (var relic in player.Relics.OfType<HappinessOfYongchunRelic>())
            ApplyFateState(relic, FateChaosRarity, 0, ChaosHighRarityBonusPercent, variant);
        Entry.Logger.Info($"[GumBlessRelic] Chaos rarity fate selected: bonus={ChaosHighRarityBonusPercent}%, variant={variant}");
    }

    private static void ApplyFateState(
        GumBlessRelic relic,
        int fateChoice,
        int personalityMask,
        int highRarityBonusPercent,
        int chaosDescriptionVariant)
    {
        relic.FateChoice = fateChoice;
        relic.PersonalityMask = personalityMask;
        relic.HighRarityBonusPercent = highRarityBonusPercent;
        relic.ChaosDescriptionVariant = chaosDescriptionVariant;
    }

    private static void ApplyFateState(
        HappinessOfYongchunRelic relic,
        int fateChoice,
        int personalityMask,
        int highRarityBonusPercent,
        int chaosDescriptionVariant)
    {
        relic.FateChoice = fateChoice;
        relic.PersonalityMask = personalityMask;
        relic.HighRarityBonusPercent = highRarityBonusPercent;
        relic.ChaosDescriptionVariant = chaosDescriptionVariant;
    }

    public static int EncodeSelection(HashSet<CardTag> selected) =>
        PersonalityTags.Select((tag, i) => selected.Contains(tag) ? 1 << i : 0).Sum();

    public static bool IsUnselectedPersonalityCard(CardModel card, Player player)
    {
        var selected = GetSelectedTags(player);
        if (selected == null) return false;
        var tags = card.Tags;

        if (tags.Contains(CultLeaderCardTags.Pure) &&
            tags.Contains(CultLeaderCardTags.Calm) &&
            tags.Contains(CultLeaderCardTags.Frenzy) &&
            tags.Contains(CultLeaderCardTags.Lively) &&
            tags.Contains(CultLeaderCardTags.Melancholy))
            return false;

        foreach (var tag in PersonalityTags.Except(selected))
        {
            if (tags.Contains(tag)) return true;
        }
        return false;
    }

    public static bool ShouldOfferOpeningSelection(Player player)
    {
        return player.Character is CultLeaderModCharacter
            && player.Relics.OfType<GumBlessRelic>().Any() && !HasFateChoice(player);
    }

    public static async Task<bool> TriggerOpeningSelection(Player player)
    {
        if (HasSelection(player)) return true;
        if (!ShouldOfferOpeningSelection(player))
            return false;
        var state = States.GetValue(player, _ => new SelectionState());
        var pending = state.Pending is { IsCompleted: false } existing
            ? existing : state.Pending = SelectForPlayer(player);
        try
        {
            return await pending;
        }
        finally
        {
            if (ReferenceEquals(state.Pending, pending))
                state.Pending = null;
        }
    }

    private static async Task<bool> SelectForPlayer(Player player)
    {
        try
        {
            // Every peer reserves the same choice ID. The native command shows UI
            // only to the owner; all other peers wait for its network result.
            Entry.Logger.Info($"[GumBlessRelic] Opening selection owner={player.NetId}, local={LocalContext.IsMe(player)}; remote peers wait for synchronized choice.");

            var runState = player.RunState;
            var cards = new List<CardModel>
            {
                runState.CreateCard<PersonalityChoicePureCard>(player),
                runState.CreateCard<PersonalityChoiceCalmCard>(player),
                runState.CreateCard<PersonalityChoiceFrenzyCard>(player),
                runState.CreateCard<PersonalityChoiceLivelyCard>(player),
                runState.CreateCard<PersonalityChoiceMelancholyCard>(player),
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
                throw new InvalidOperationException($"Expected 2 selected cards, got {selectedCards.Count}.");
            }

            var selectedTags = selectedCards
                .Select(GetPersonalityTag)
                .Where(tag => tag.HasValue)
                .Select(tag => tag!.Value)
                .ToHashSet();

            if (selectedTags.Count != 2)
                throw new InvalidOperationException("Expected two distinct personality tags.");
            SetSelection(player, selectedTags, FatePredetermined);
            Entry.Logger.Info($"[GumBlessRelic] Opening selection complete: {string.Join(", ", selectedTags)}");
            return true;
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[GumBlessRelic] Opening selection failed: {ex}");
            return false;
        }
    }

    private static CardTag? GetPersonalityTag(CardModel card)
    {
        var typeName = card.GetType().Name;
        if (typeName == nameof(PersonalityChoicePureCard)) return CultLeaderCardTags.Pure;
        if (typeName == nameof(PersonalityChoiceCalmCard)) return CultLeaderCardTags.Calm;
        if (typeName == nameof(PersonalityChoiceFrenzyCard)) return CultLeaderCardTags.Frenzy;
        if (typeName == nameof(PersonalityChoiceLivelyCard)) return CultLeaderCardTags.Lively;
        if (typeName == nameof(PersonalityChoiceMelancholyCard)) return CultLeaderCardTags.Melancholy;
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

    public LocString? SelectedPersonalityDescription => GetSelectionDescription(this);

    internal static LocString? GetSelectionDescription(RelicModel relic)
    {
        if (!relic.IsMutable || relic.Owner == null)
            return null;

        var upgraded = relic is HappinessOfYongchunRelic;
        var fateChoice = relic switch
        {
            GumBlessRelic starter => starter.FateChoice,
            HappinessOfYongchunRelic happiness => happiness.FateChoice,
            _ => FateUnchosen
        };

        if (fateChoice == FateChaosRarity)
        {
            var variant = relic switch
            {
                GumBlessRelic starter => starter.ChaosDescriptionVariant,
                HappinessOfYongchunRelic happiness => happiness.ChaosDescriptionVariant,
                _ => 0
            };
            variant = Math.Clamp(variant, 0, ChaosDescriptions.Length - 1);
            var chaosKey = $"CULT_LEADER_CHAOS_FATE_{(upgraded ? "UPGRADED_" : "")}{variant}.description";
            LocManager.Instance.GetTable("relics").MergeWith(new Dictionary<string, string>
            {
                [chaosKey] = ChaosDescriptions[variant]
            });
            return new LocString("relics", chaosKey);
        }

        if (GetSelectedTags(relic.Owner) is not { Count: 2 } selected)
            return null;

        var names = PersonalityTags.Where(selected.Contains).Select(GetPersonalityName).ToList();
        var key = $"CULT_LEADER_PERSONALITY_SELECTION_{(upgraded ? "UPGRADED_" : "")}{EncodeSelection(selected)}.description";
        LocManager.Instance.GetTable("relics").MergeWith(new Dictionary<string, string>
        {
            [key] = upgraded
                ? $"{names[0]}和{names[1]}使徒的出现概率提升，拾起时获得2次稀有卡牌奖励。"
                : $"{names[0]}和{names[1]}使徒的出现概率提升。"
        });
        return new LocString("relics", key);
    }

    public static CardRarity ApplyMerchantRarityBonus(Player player, CardRarity rarity)
    {
        var bonus = GetHighRarityBonusPercent(player);
        if (rarity != CardRarity.Common || bonus <= 0
            || player.PlayerRng.Shops.NextInt(100) >= bonus)
            return rarity;

        return player.PlayerRng.Shops.NextBool()
            ? CardRarity.Uncommon
            : CardRarity.Rare;
    }

    /// <summary>
    /// Filter a list of cards, removing unselected personality cards (85% rejection rate).
    /// Returns a new list; if no cards were filtered, returns the original list.
    /// </summary>
    public static List<CardModel> FilterUnselectedCards(List<CardModel> cards, Player player, MegaCrit.Sts2.Core.Random.Rng? rng = null)
    {
        if (!HasSelection(player)) return cards;
        rng ??= player.PlayerRng.Rewards;

        var filtered = new List<CardModel>(cards.Count);
        bool changed = false;

        foreach (var card in cards)
        {
            if (IsUnselectedPersonalityCard(card, player))
            {
                // 85% chance to reject unselected personality cards
                if (rng.NextDouble() >= 0.85)
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
            if (player != Owner || !HasSelection(player))
            {
                Entry.Logger.Info("[GumBlessRelic] ModifyCardRewardCreationOptions: selection not made, returning original");
                return options;
            }

            var existingFilter = options.CardPoolFilter;

            return options.WithFilter(card =>
            {
                try
                {
                    if (existingFilter != null && !existingFilter(card)) return false;

                    if (IsUnselectedPersonalityCard(card, player))
                    {
                        return player.PlayerRng.Rewards.NextDouble() >= 0.85;
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

    public override CardRarity ModifyMerchantCardRarity(Player player, CardRarity rarity) =>
        player == Owner ? ApplyMerchantRarityBonus(player, rarity) : rarity;
}


