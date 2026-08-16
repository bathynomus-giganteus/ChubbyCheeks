using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class SwooshGlovesRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/swoosh_gloves.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/swoosh_gloves.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/swoosh_gloves.png";

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner.Creature))
            return;

        int currentFrenzy = ApostleCardEffectHelpers.GetFrenzyResourceAmount(base.Owner.Creature);
        decimal amount = currentFrenzy <= 0 ? 3m : 1m;

        Flash();
        await ApostleCardPlayHelpers.ApplyFrenzyPower(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            amount,
            base.Owner.Creature,
            null
        );
    }
}
