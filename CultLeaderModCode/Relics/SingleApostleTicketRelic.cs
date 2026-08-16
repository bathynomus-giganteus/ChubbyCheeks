using System.Linq;
using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
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
            .ToList();

        var weightedPool = GumBlessRelic.FilterUnselectedCards(allApostleCards);
        if (weightedPool.Count == 0)
            return;

        var canonicalCard = weightedPool[Random.Shared.Next(weightedPool.Count)];
        var card = base.Owner.RunState.CreateCard(canonicalCard, base.Owner);
        var addResult = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd([addResult], 2f);
    }
}
