using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Adjust. Each time the owner gains Retain or Happiness, deal the stored damage to a random enemy,
/// then increase that damage by 3 for the rest of this combat.
/// </summary>
[RegisterPower]
public class AdjustPower : ModPowerTemplate
{
    private sealed class Data
    {
        public decimal Damage = 3m;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/adjust.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/adjust.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

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
        if (power is not RetainPower and not HappinessPower)
            return;

        var target = ApostleCardEffectHelpers.RandomEnemy(base.Owner);
        if (target == null)
            return;

        var data = GetInternalData<Data>();
        await CreatureCmd.Damage(
            choiceContext,
            target,
            data.Damage,
            ValueProp.Unpowered,
            base.Owner,
            null,
            null
        );
        data.Damage += 3m;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        GetInternalData<Data>().Damage = 3m;
        await base.AfterCombatEnd(room);
    }
}