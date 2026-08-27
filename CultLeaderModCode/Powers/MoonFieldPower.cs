using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Moon Field. Attacks against debuffed enemies deal bonus damage for each Moon Field stack and debuff type.
/// </summary>
[RegisterPower]
public class MoonFieldPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/moon_field.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/moon_field.png";

    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        decimal result = base.ModifyHpLostBeforeOsty(target, amount, props, dealer, cardSource);
        if (
            base.Owner == null
            || dealer != base.Owner
            || base.Amount <= 0m
            || result <= 0m
            || !props.IsPoweredAttack()
            || cardSource?.Type != CardType.Attack
        )
            return result;

        int debuffTypes = ApostleCardEffectHelpers.CountDebuffTypes(target);
        return debuffTypes > 0
            ? result + base.Amount * debuffTypes
            : result;
    }
}
