using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using System.Linq;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public abstract class CultLeaderModRelic : ModRelicTemplate
{
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.Any(player => player.Character is CultLeaderModCharacter);
    }

    public override bool IsAllowedAtNeow(Player player)
    {
        return player.Character is CultLeaderModCharacter;
    }
}
