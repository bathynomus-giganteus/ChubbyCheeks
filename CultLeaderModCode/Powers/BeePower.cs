using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Happy Bee summon. At the end of the player's turn, deals damage equal to its stacks
/// to a random enemy, then loses one stack. Bees do not block damage.
/// </summary>
[RegisterPower]
public class BeePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_26.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_26.png";

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side != CombatSide.Player || base.Owner == null || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        var target = ApostleCardEffectHelpers.RandomEnemy(base.Owner);
        if (target != null)
        {
            await CreatureCmd.Damage(
                choiceContext,
                target,
                base.Amount,
                ValueProp.Unpowered,
                base.Owner
            );
        }

        await PowerCmd.Decrement(this);
    }
}
