using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class GoldenCrayonRelic : CultLeaderModRelic
{
    private const int ThresholdValue = 5;

    private int _combatCount;

    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/golden_crayon.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/golden_crayon.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/golden_crayon.png";

    public override bool ShowCounter => true;

    public override int DisplayAmount => CombatCount % ThresholdValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Threshold", ThresholdValue)];

    [SavedProperty]
    public int CombatCount
    {
        get => _combatCount;
        set
        {
            AssertMutable();
            _combatCount = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        CombatCount++;
        return Task.CompletedTask;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (room is not CombatRoom || player != base.Owner || CombatCount % ThresholdValue != 0)
            return false;

        var deck = PileType.Deck.GetPile(base.Owner).Cards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle) && card.IsUpgradable)
            .ToList();

        if (deck.Count == 0)
            return false;

        rewards.Add(new GoldenCrayonUpgradeReward(base.Owner, Flash));
        return true;
    }
}

public sealed class GoldenCrayonUpgradeReward : Reward
{
    private readonly Action _onClaimed;

    public GoldenCrayonUpgradeReward(Player player, Action onClaimed) : base(player)
    {
        _onClaimed = onClaimed;
    }

    protected override RewardType RewardType => RewardType.SpecialCard;

    public override int RewardsSetIndex => 6;

    public override LocString Description => new LocString("gameplay_ui", "CULT_LEADER_GOLDEN_CRAYON.prompt");

    protected override string? IconPath => "res://CultLeaderMod/images/relics/golden_crayon.png";

    public override bool IsPopulated => true;

    public override void Populate()
    {
    }

    protected override async Task<bool> OnSelect()
    {
        var deck = PileType.Deck.GetPile(Player).Cards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle) && card.IsUpgradable)
            .ToList();

        if (deck.Count == 0)
            return false;

        var prefs = new CardSelectorPrefs(
            new LocString("gameplay_ui", "CULT_LEADER_GOLDEN_CRAYON.prompt"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };

        var selected = await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            deck,
            Player,
            prefs);

        var selectedList = selected.ToList();
        if (selectedList.Count == 0)
            return false;

        _onClaimed?.Invoke();
        CardCmd.Upgrade(selectedList, CardPreviewStyle.HorizontalLayout);
        return true;
    }

    public override void MarkContentAsSeen()
    {
    }
}
