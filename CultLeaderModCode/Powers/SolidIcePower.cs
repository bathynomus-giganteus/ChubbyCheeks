using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class SolidIcePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/solidice.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/solidice.png";

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (target == base.Owner && cardSource != null && base.Amount > 0)
            return base.Amount;

        return base.ModifyBlockAdditive(target, block, props, cardSource, cardPlay);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        await CreatureCmd.GainBlock(base.Owner, base.Amount, ValueProp.Move, null, true);
    }

    public async Task TriggerActive(PlayerChoiceContext choiceContext, Creature? applier, CardModel? cardSource)
    {
        if (base.Amount <= 0)
            return;

        var blockToGain = base.Amount;
        await CreatureCmd.GainBlock(base.Owner, blockToGain, ValueProp.Move, null, true);
    }
}
