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
/// 要来见少女吗？ — 获得活力/治愈时，获得另一种增益的一半，且不会递归触发。
/// </summary>
[RegisterPower]
public class FrenzyOnHealPower : ModPowerTemplate
{
    private bool _isConverting;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (_isConverting || Owner == null || power.Owner != Owner || amount <= 0m || Amount <= 0m)
            return;

        var gainedVigor = power is VigorPower or FervorPower;
        var gainedHealing = power is HealingPower or LifeEssencePower;
        if (!gainedVigor && !gainedHealing)
            return;

        var convertedAmount = Math.Floor(amount * Amount / 2m);
        if (convertedAmount <= 0m)
            return;

        _isConverting = true;
        try
        {
            if (gainedVigor)
            {
                await ApostlePowerRules.ApplyApostlePower<HealingPower, LifeEssencePower>(
                    choiceContext,
                    Owner,
                    convertedAmount,
                    Owner,
                    null);
            }
            else
            {
                await ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>(
                    choiceContext,
                    Owner,
                    convertedAmount,
                    Owner,
                    null);
            }
        }
        finally
        {
            _isConverting = false;
        }
    }
}
