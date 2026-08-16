using System.Linq;
using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class EmiliasEpadClassicRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/emilias_epad_classic.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/emilias_epad_classic.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/emilias_epad_classic.png";

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != CombatSide.Enemy)
            return;

        var playerCombatState = base.Owner.PlayerCombatState;
        if (playerCombatState == null)
            return;

        int calmCount = playerCombatState.AllCards
            .Count(card => card.Tags.Contains(CultLeaderCardTags.Calm));
        decimal amount = 1m + calmCount / 10m;

        Flash();
        await ApostleCardPlayHelpers.ApplyCalmPower(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            amount,
            base.Owner.Creature,
            null
        );
    }
}
