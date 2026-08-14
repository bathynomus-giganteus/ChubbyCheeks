using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Love Energy. Every hit this enemy lands on a player (blocked or not) makes it lose Strength.
/// </summary>
[RegisterPower]
public class LoveEnergyPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/love_energy.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/love_energy.png";

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (base.Amount <= 0 || dealer == null || dealer != base.Owner)
            return;
        if (target == null || !target.IsPlayer || result.TotalDamage <= 0 || !props.IsPoweredAttack())
            return;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            base.Owner,
            -(decimal)base.Amount,
            base.Applier ?? target,
            cardSource
        );
    }
}