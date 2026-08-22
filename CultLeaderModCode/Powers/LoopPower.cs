using CultLeaderMod.CultLeaderModCode.Cards;
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
/// Loop. Whenever the owner gains Healing, Vigor, Plating, Retain, or Bitter Pain,
/// deal this power's stacks as damage to all enemies.
/// </summary>
[RegisterPower]
public class LoopPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/loop.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/loop.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount <= 0m || base.Owner == null || power.Owner != base.Owner)
            return;
        if (!IsTrackedPower(power))
            return;

        var enemies = ApostleCardEffectHelpers.AliveEnemies(base.Owner);
        if (enemies.Count == 0)
            return;

        decimal damage = base.Amount * amount;
        foreach (var enemy in enemies)
        {
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                damage,
                ValueProp.Unpowered,
                base.Owner,
                null,
                null
            );
        }
    }

    private static bool IsTrackedPower(PowerModel power)
    {
        return power is HealingPower
            or LifeEssencePower
            or VigorPower
            or FervorPower
            or PlatingPower
            or SolidIcePower
            or RetainPower
            or HappinessPower
            or BitterPainPower
            or BitterPainBurstPower;
    }
}
