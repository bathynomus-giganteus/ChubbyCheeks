using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class HolyPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/holy.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/holy.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (base.Amount <= 0 || power.Owner != base.Owner || amount < 3m || !IsTrackedResource(power))
            return;

        await MegaCrit.Sts2.Core.Commands.PowerCmd.ModifyAmount(
            choiceContext,
            this,
            1m,
            base.Owner,
            cardSource,
            silent: true);
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (base.Amount > 0
            && dealer == base.Owner
            && target != null
            && target.Side != base.Owner.Side
            && (cardSource != null || props.HasFlag(ValueProp.Unpowered)))
        {
            return base.Amount;
        }

        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
    }

    private static bool IsTrackedResource(PowerModel power) =>
        power is HealingPower or LifeEssencePower
            or PlatingPower or SolidIcePower
            or VigorPower or FervorPower
            or RetainPower or HappinessPower
            or BitterPainPower or BitterPainBurstPower;
}
