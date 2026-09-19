using System.Linq;
using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class SingleApostleTicketRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/single_apostle_ticket.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/single_apostle_ticket.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/single_apostle_ticket.png";

    public override async Task AfterObtained()
    {
        var allApostleCards = ModelDb.AllCards
            .Where(card => card.Tags.Contains(CultLeaderCardTags.Apostle))
            .Where(card => card.CanBeGeneratedInCombat)
            .OrderBy(card => card.Id.ToString(), StringComparer.Ordinal)
            .ToList();

        var weightedPool = GumBlessRelic.FilterUnselectedCards(allApostleCards, Owner);
        var candidates = weightedPool
            .OrderBy(_ => Owner.PlayerRng.Rewards.NextInt())
            .Take(10)
            .Select(card => Owner.RunState.CreateCard(card, Owner))
            .ToList();
        if (candidates.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        var selected = (await CardSelectCmd.FromSimpleGrid(
            new BlockingPlayerChoiceContext(),
            candidates,
            Owner,
            prefs)).SingleOrDefault();
        if (selected == null)
            return;

        var addResult = await CardPileCmd.Add(selected, PileType.Deck);
        CardCmd.PreviewCardPileAdd([addResult], 2f);
    }
}
