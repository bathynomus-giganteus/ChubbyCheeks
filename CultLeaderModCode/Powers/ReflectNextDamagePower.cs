using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Next damage reflect. The next time the owner takes unblocked attack damage,
/// reflect that damage to the attacker, then remove this power.
/// </summary>
[RegisterPower]
public class ReflectNextDamagePower : ModPowerTemplate
{
    private sealed class Data
    {
        public bool Consumed;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/reflect_next_damage.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/reflect_next_damage.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (GetInternalData<Data>().Consumed)
            return;
        if (target != base.Owner || dealer == null || dealer == base.Owner || result.TotalDamage <= 0m || !props.IsPoweredAttack())
            return;

        GetInternalData<Data>().Consumed = true;

        await CreatureCmd.Damage(
            choiceContext,
            dealer,
            result.TotalDamage,
            ValueProp.Unpowered,
            base.Owner
        );
        await PowerCmd.Remove(this);
    }
}
