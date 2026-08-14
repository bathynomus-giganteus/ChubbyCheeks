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
/// Mayor Lonne. Every N debuff stacks applied to enemies grants 1 energy next turn.
/// </summary>
[RegisterPower]
public class DebuffApplyCounterPower : ModPowerTemplate
{
    private sealed class Data
    {
        public int Applied;
        public int Threshold;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_10.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_10.png";

    public override int DisplayAmount
    {
        get
        {
            var data = GetInternalData<Data>();
            if (data.Threshold <= 0)
                return 0;
            return data.Threshold - (data.Applied % data.Threshold);
        }
    }

    protected override object InitInternalData()
    {
        return new Data { Threshold = (int)Amount };
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        GetInternalData<Data>().Threshold = (int)Amount;
        InvokeDisplayAmountChanged();
        await base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount <= 0m || base.Owner == null)
            return;
        if (power.Type != PowerType.Debuff || power.Owner == null || !power.Owner.IsMonster)
            return;

        var data = GetInternalData<Data>();
        data.Applied += (int)amount;
        if (data.Threshold <= 0)
            return;

        int triggers = data.Applied / data.Threshold;
        if (triggers <= 0)
        {
            InvokeDisplayAmountChanged();
            return;
        }

        data.Applied %= data.Threshold;
        for (int i = 0; i < triggers; i++)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(
                choiceContext,
                base.Owner,
                1m,
                base.Owner,
                cardSource
            );
        }

        InvokeDisplayAmountChanged();
    }
}