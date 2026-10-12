using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Character;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.addons.mega_text;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class RepresentativeChanceRelic : CultLeaderModRelic
{
    public const int MaxUses = 3;
    private readonly HashSet<ModelId> _recruitedThisCombat = [];
    private bool _claimingRecruit;
    private int _pendingCombatCardRewards;
    private int _remainingUses;

    [SavedProperty]
    public int RemainingUses
    {
        get => _remainingUses;
        set
        {
            AssertMutable();
            _remainingUses = Math.Clamp(value, 0, MaxUses);
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public bool Initialized { get; set; }

    [SavedProperty]
    public List<SerializableCard> RecruitedCards
    {
        get => _recruitedThisCombat
            .OrderBy(id => id.ToString(), StringComparer.Ordinal)
            .Select(id => new SerializableCard { Id = id })
            .ToList();
        set
        {
            _recruitedThisCombat.Clear();
            foreach (var card in value)
            {
                if (card.Id is { } id)
                    _recruitedThisCombat.Add(id);
            }
        }
    }

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => RemainingUses;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/chance.jpg";
    public override string? CustomBigIconPath => CustomIconPath;
    public override string? CustomIconOutlinePath => CustomIconPath;

    public static RepresentativeChanceRelic? GetFor(Player player) =>
        player.Relics.OfType<RepresentativeChanceRelic>().FirstOrDefault();

    public override async Task AfterObtained()
    {
        if (Initialized)
            return;

        Initialized = true;
        RemainingUses = MaxUses;
        var starters = PileType.Deck.GetPile(Owner).Cards
            .Where(card => card is CultLeaderStrike or CultLeaderDefend
                or ApostleAttackCard or TestAddApostleCards)
            .ToList();
        if (starters.Count == 0)
            return;

        await CardPileCmd.RemoveFromDeck(starters, showPreview: false);
        var tickets = Enumerable.Range(0, starters.Count)
            .Select(_ => Owner.RunState.CreateCard<TestAddApostleCards>(Owner))
            .ToList();
        await CardPileCmd.Add(tickets, PileType.Deck, skipVisuals: true);
    }

    public void RecordRecruit(CardModel card)
    {
        if (RemainingUses > 0 && card.Tags.Contains(CultLeaderCardTags.Apostle))
            _recruitedThisCombat.Add(card.Id);
    }

    public void TransferRecruitsTo(RepresentativeChanceRelic replacement)
    {
        foreach (var id in _recruitedThisCombat)
            replacement._recruitedThisCombat.Add(id);
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        _recruitedThisCombat.Clear();
        _pendingCombatCardRewards = 0;
        return Task.CompletedTask;
    }

    public override async Task BeforeCombatRewardOffered(RewardsSet rewards, CombatRoom room)
    {
        if (rewards.Player != Owner || _pendingCombatCardRewards <= 0)
            return;

        var rewardCount = _pendingCombatCardRewards;
        _pendingCombatCardRewards = 0;
        var tickets = Enumerable.Range(0, rewardCount)
            .Select(_ => Owner.RunState.CreateCard<TestAddApostleCards>(Owner))
            .ToList();
        var results = await CardPileCmd.Add(
            tickets,
            PileType.Deck,
            CardPilePosition.Bottom,
            this);
        var successfulResults = results.Where(result => result.success).ToList();
        if (successfulResults.Count == 0)
            return;

        Flash();
        CardCmd.PreviewCardPileAdd(successfulResults, 1.2f);
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player, List<CardCreationResult> options, CardCreationOptions creationOptions)
    {
        if (player != Owner)
            return false;
        return ReplaceApostleOffers(player, options);
    }

    public override void ModifyMerchantCardCreationResults(
        Player player, List<CardCreationResult> cards)
    {
        if (player == Owner)
            ReplaceApostleOffers(player, cards);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        if (card.Owner != Owner || !card.Tags.Contains(CultLeaderCardTags.Apostle)
            || _claimingRecruit)
            return false;

        newCard = CreateRecruitmentCard(Owner, card.IsUpgraded);
        return true;
    }

    private bool ReplaceApostleOffers(Player player, List<CardCreationResult> results)
    {
        var changed = false;
        foreach (var result in results)
        {
            if (!result.Card.Tags.Contains(CultLeaderCardTags.Apostle))
                continue;

            result.ModifyCard(CreateRecruitmentCard(player, result.Card.IsUpgraded), this);
            changed = true;
        }
        return changed;
    }

    private static TestAddApostleCards CreateRecruitmentCard(Player player, bool upgraded)
    {
        var card = player.RunState.CreateCard<TestAddApostleCards>(player);
        if (upgraded)
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        return card;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner || room is not CombatRoom)
            return false;

        var changed = false;
        if (RemainingUses > 0 && _recruitedThisCombat.Count > 0 &&
            (GetDeckTickets(player).Any() || rewards.Any(reward => reward is CardReward)))
        {
            rewards.Add(new RecruitmentResultsReward(player, this));
            changed = true;
        }

        return changed;
    }

    public override bool TryModifyRewardsLate(
        Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner || room is not CombatRoom)
            return false;

        _pendingCombatCardRewards = rewards.RemoveAll(reward => reward is CardReward);
        return _pendingCombatCardRewards > 0;
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;
        options.Add(new TendFireRestSiteOption(player));
        return true;
    }

    public void Refill()
    {
        RemainingUses = MaxUses;
        Flash();
    }

    internal IReadOnlyList<CardModel> GetRecruitmentResults() =>
        ModelDb.AllCards.Where(card => _recruitedThisCombat.Contains(card.Id))
            .OrderBy(card => card.Id.ToString(), StringComparer.Ordinal)
            .Select(card => Owner.RunState.CreateCard(card, Owner))
            .ToList();

    internal async Task<bool> ClaimRecruit(CardModel selected)
    {
        if (RemainingUses <= 0 || !_recruitedThisCombat.Contains(selected.Id))
            return false;

        var tickets = GetDeckTickets(Owner).ToList();
        if (tickets.Count == 0)
            return false;

        var ticket = tickets[Owner.PlayerRng.Rewards.NextInt(tickets.Count)];
        var gained = Owner.RunState.CreateCard(selected.CanonicalInstance, Owner);
        _claimingRecruit = true;
        try
        {
            if (!(await CardPileCmd.Add(gained, PileType.Deck)).success)
                return false;
        }
        finally
        {
            _claimingRecruit = false;
        }

        await CardPileCmd.RemoveFromDeck(ticket, showPreview: false);
        RemainingUses--;
        Flash();
        return true;
    }

    private static IEnumerable<TestAddApostleCards> GetDeckTickets(Player player)
    {
        var tickets = PileType.Deck.GetPile(player).Cards
            .OfType<TestAddApostleCards>().ToList();
        var normal = tickets.Where(card => !card.IsUpgraded).ToList();
        return normal.Count > 0 ? normal : tickets.Where(card => card.IsUpgraded);
    }
}

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class DoubleRepresentativeChanceRelic : RepresentativeChanceRelic
{
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/double_chance.png";

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            var tickets = PileType.Deck.GetPile(Owner).Cards
                .OfType<TestAddApostleCards>()
                .Where(card => card.IsUpgradable).ToList();
            CardCmd.Upgrade(tickets, CardPreviewStyle.None);
        }
        return base.BeforeRoomEntered(room);
    }
}

public sealed class RecruitmentResultsReward : Reward
{
    private readonly RepresentativeChanceRelic _relic;

    public RecruitmentResultsReward(Player player, RepresentativeChanceRelic relic) : base(player)
    {
        _relic = relic;
    }

    protected override RewardType RewardType => RewardType.RemoveCard;
    public override int RewardsSetIndex => 6;
    public override LocString Description =>
        new("gameplay_ui", "CULT_LEADER_RECRUITMENT_RESULTS.prompt");
    protected override string? IconPath => "res://CultLeaderMod/images/relics/single_apostle_ticket.png";
    public override bool IsPopulated => true;
    public override void Populate() { }

    protected override async Task<bool> OnSelect()
    {
        var options = _relic.GetRecruitmentResults();
        if (options.Count == 0)
            return false;

        var prefs = new CardSelectorPrefs(Description, 0, 1)
        {
            RequireManualConfirmation = true
        };
        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(), options, Player, prefs)).SingleOrDefault();
        return selected != null && await _relic.ClaimRecruit(selected);
    }

    public override void MarkContentAsSeen() { }

    public override SerializableReward ToSerializable() => new()
    {
        RewardType = RewardType.RemoveCard,
        PredeterminedModelId = _relic.Id
    };
}

[HarmonyPatch]
public static class RecruitmentResultsSkipButtonPatch
{
    [HarmonyPatch(typeof(NSimpleCardSelectScreen), "_Ready")]
    [HarmonyPostfix]
    private static void AfterReady(NSimpleCardSelectScreen __instance,
        CardSelectorPrefs ____prefs, HashSet<CardModel> ____selectedCards)
    {
        UpdateLabel(__instance, ____prefs, ____selectedCards.Count);
    }

    [HarmonyPatch(typeof(NSimpleCardSelectScreen), "OnCardClicked")]
    [HarmonyPostfix]
    private static void AfterCardClicked(NSimpleCardSelectScreen __instance,
        CardSelectorPrefs ____prefs, HashSet<CardModel> ____selectedCards)
    {
        UpdateLabel(__instance, ____prefs, ____selectedCards.Count);
    }

    private static void UpdateLabel(NSimpleCardSelectScreen screen, CardSelectorPrefs prefs, int selectedCount)
    {
        if (prefs.Prompt.LocTable != "gameplay_ui" ||
            prefs.Prompt.LocEntryKey != "CULT_LEADER_RECRUITMENT_RESULTS.prompt")
            return;

        var button = screen.GetNode<NConfirmButton>("%Confirm");
        var label = FindLabel(button);
        if (label == null)
            return;

        var key = selectedCount == 0 ? "CULT_LEADER_RECRUITMENT_RESULTS.skip"
            : "CULT_LEADER_RECRUITMENT_RESULTS.confirm";
        var caption = new LocString("gameplay_ui", key).GetFormattedText();
        if (label is MegaLabel megaLabel)
            megaLabel.SetTextAutoSize(caption);
        else
            label.Text = caption;
    }

    private static Label? FindLabel(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is NHotkeyIcon)
                continue;
            if (child is Label label)
                return label;
            if (FindLabel(child) is { } nested)
                return nested;
        }
        return null;
    }
}

[HarmonyPatch(typeof(Reward), nameof(Reward.FromSerializable))]
public static class RecruitmentResultsRewardSavePatch
{
    [HarmonyPrefix]
    private static bool Prefix(SerializableReward save, Player player, ref Reward __result)
    {
        if (save.RewardType != RewardType.RemoveCard ||
            (save.PredeterminedModelId != ModelDb.Relic<RepresentativeChanceRelic>().Id &&
             save.PredeterminedModelId != ModelDb.Relic<DoubleRepresentativeChanceRelic>().Id))
            return true;

        var relic = RepresentativeChanceRelic.GetFor(player);
        if (relic == null)
            return true;

        __result = new RecruitmentResultsReward(player, relic);
        return false;
    }
}
