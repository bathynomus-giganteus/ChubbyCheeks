using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Relics;

public sealed class CultLeaderStartingRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsStackable => true;
    public override bool ShowCounter => false;

    private int SelectedPersonalityMask => StackCount - 1;

    public bool IsPersonalitySelected(ApostlePersonality personality) =>
        (SelectedPersonalityMask & (1 << (int)personality)) != 0;

    public override async Task AfterActEntered()
    {
        if (SelectedPersonalityMask != 0 || Owner.RunState.CurrentActIndex != 0)
            return;

        IReadOnlyList<CardModel> personalityCards =
        [
            Owner.RunState.CreateCard(ModelDb.Card<PurePersonalityChoice>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<CalmPersonalityChoice>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<FanaticPersonalityChoice>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<MelancholyPersonalityChoice>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<LivelyPersonalityChoice>(), Owner)
        ];

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 2)
        {
            RequireManualConfirmation = true
        };
        IEnumerable<CardModel> selected = await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            personalityCards,
            Owner,
            prefs);

        int mask = selected
            .OfType<IPersonalityChoice>()
            .Aggregate(0, (current, card) => current | (1 << (int)card.Personality));

        for (int i = 0; i < mask; i++)
            IncrementStackCount();

    }

    public override IEnumerable<CardModel> ModifyMerchantCardPool(
        Player player,
        IEnumerable<CardModel> options)
    {
        if (player != Owner || SelectedPersonalityMask == 0)
            return options;

        return ApplyPersonalityWeights(options);
    }

    public IEnumerable<CardModel> ApplyPersonalityWeights(IEnumerable<CardModel> cards)
    {
        foreach (CardModel card in cards)
        {
            int weight = card is IApostleCard apostle && !IsPersonalitySelected(apostle.Personality) ? 1 : 6;
            for (int i = 0; i < weight; i++)
                yield return card;
        }
    }

}
