using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 要来见少女吗？ — 每次恢复1点生命或获得1层治愈时，获得1层活力；埃尔德形态下获得狂热。
/// </summary>
[RegisterPower]
public class FrenzyOnHealPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        await base.AfterCurrentHpChanged(creature, delta);

        if (delta <= 0m || Owner == null || creature != Owner || !creature.IsPlayer || Amount <= 0m)
            return;

        await GainVigor(choiceContext: new ThrowingPlayerChoiceContext(), delta);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (Owner == null || power.Owner != Owner || power is not HealingPower || amount <= 0m || Amount <= 0m)
            return;

        await GainVigor(choiceContext, amount);
    }

    private async Task GainVigor(PlayerChoiceContext choiceContext, decimal triggerAmount)
    {
        if (Owner == null || triggerAmount <= 0m || Amount <= 0m)
            return;

        await ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>(
            choiceContext,
            Owner,
            triggerAmount * Amount,
            Owner,
            null
        );
    }
}
