using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class LittleWishBecomesLightPower : ModPowerTemplate
{
    [SavedProperty]
    public bool PreventHpLossUntilTurnStart { get; set; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/wish_light.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/wish_light.png";

    public override bool ShouldDie(Creature creature)
    {
        if (creature != base.Owner)
            return true;

        return !PreventHpLossUntilTurnStart && (base.Owner.GetPower<HolyPower>()?.Amount ?? 0) <= 0;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        await base.AfterPreventingDeath(creature);
        if (creature != base.Owner || PreventHpLossUntilTurnStart)
            return;

        var holy = base.Owner.GetPower<HolyPower>();
        var holyAmount = holy?.Amount ?? 0;
        if (holy == null || holyAmount <= 0)
            return;

        foreach (var debuff in base.Owner.Powers
                     .Where(power => power.TypeForCurrentAmount == PowerType.Debuff)
                     .ToList())
        {
            await PowerCmd.Remove(debuff);
        }

        await PowerCmd.Remove(holy);
        await CreatureCmd.Heal(base.Owner, holyAmount);
        PreventHpLossUntilTurnStart = true;
        Flash();
    }

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return target == base.Owner && PreventHpLossUntilTurnStart ? 0m : amount;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);
        if (PreventHpLossUntilTurnStart && side == base.Owner.Side && participants.Contains(base.Owner))
            PreventHpLossUntilTurnStart = false;
    }
}
