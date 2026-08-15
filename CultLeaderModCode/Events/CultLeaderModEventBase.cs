using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Events;

public abstract class CultLeaderModEventBase : ModEventTemplate
{
    protected bool HasSeenEvent(IRunState runState)
    {
        return runState.MapPointHistory
            .SelectMany(act => act)
            .SelectMany(entry => entry.Rooms)
            .Any(room => room.RoomType == RoomType.Event && room.ModelId == Id);
    }

    protected bool HasEnoughPersonalityCards(IRunState runState, CardTag personalityTag)
    {
        return runState.Players.Any(player =>
            player.Deck.Cards.Count(card => card.Tags.Contains(personalityTag)) >= 5);
    }
}