using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Extant. Whenever this enemy takes attack damage, it also takes damage equal to
/// (current player Retain stacks) for each stack of Extant.
/// </summary>
[RegisterPower]
public class ExtantPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/active.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/active.png";

    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != base.Owner || base.Amount <= 0 || amount <= 0m || !props.IsPoweredAttack())
            return amount;

        decimal retain = CountRetain(dealer);
        if (retain <= 0m)
            return amount;

        return amount + (base.Amount * retain);
    }

    private static decimal CountRetain(Creature? creature)
    {
        if (creature == null)
            return 0m;

        return (creature.GetPower<RetainPower>()?.Amount ?? 0m)
             + (creature.GetPower<HappinessPower>()?.Amount ?? 0m);
    }
}