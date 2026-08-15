using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Fate Clock. At the start of this enemy's next turn, it takes damage equal to its stacks, then the power is removed.
/// </summary>
[RegisterPower]
public class FateClockPower : ModPowerTemplate, IPowerExtraIconAmountLabelSpecsProvider
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Damage", 0m)];

    public override int DisplayAmount => (int)DynamicVars["Damage"].BaseValue;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/clock.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/clock.png";

    public void SetDamage(decimal damage)
    {
        DynamicVars["Damage"].BaseValue = damage;
        InvokeDisplayAmountChanged();
    }

    public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
    {
        return
        [
            ExtraIconAmountLabelSpec.Plain(
                ExtraIconAmountLabelCorner.TopLeft,
                Amount.ToString())
        ];
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        if (side != CombatSide.Enemy || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        Flash();

        int remaining = base.Amount - 1;
        if (remaining <= 0)
        {
            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                base.Owner,
                DynamicVars["Damage"].BaseValue,
                ValueProp.Unpowered,
                base.Applier ?? base.Owner
            );
            await PowerCmd.Remove(this);
        }
        else
        {
            base.SetAmount(remaining);
        }
    }
}
