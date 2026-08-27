using CultLeaderMod.CultLeaderModCode.Cards;
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
/// 向前迈进的决心：每消耗到阈值层数的活力/狂热，就获得2层活力。
/// Instanced so each copy of the card tracks its own counter.
/// </summary>
[RegisterPower]
public class ForwardResolvePower : ModPowerTemplate
{
    private sealed class Data
    {
        public int Consumed;
        public int Triggers;
        public int Threshold;
        public int VigorGainPerTrigger = 2;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_19.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_19.png";

    public override int DisplayAmount
    {
        get
        {
            var data = GetInternalData<Data>();
            if (data.Threshold <= 0)
                return 0;
            return data.Threshold - (data.Consumed % data.Threshold);
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

    public void ConfigureVigorGain(int amount)
    {
        GetInternalData<Data>().VigorGainPerTrigger = Math.Max(0, amount);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (amount >= 0m || Owner == null)
            return;

        if ((power is VigorPower || power is FervorPower) && power.Owner == Owner)
        {
            var data = GetInternalData<Data>();
            data.Consumed += (int)(-amount);

            int newTriggers = data.Threshold <= 0 ? 0 : data.Consumed / data.Threshold;
            int triggerDelta = newTriggers - data.Triggers;
            if (triggerDelta <= 0)
            {
                InvokeDisplayAmountChanged();
                return;
            }

            data.Triggers = newTriggers;
            InvokeDisplayAmountChanged();
            Flash();
            await ApostleCardPlayHelpers.ApplyFrenzyPower(
                choiceContext,
                Owner,
                triggerDelta * data.VigorGainPerTrigger,
                Owner,
                cardSource
            );
        }
    }
}
