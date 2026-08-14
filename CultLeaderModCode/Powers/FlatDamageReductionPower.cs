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
/// Snow Mist. While the owner has Block, incoming attack damage is reduced by this power's stacks.
/// </summary>
[RegisterPower]
public class FlatDamageReductionPower : ModPowerTemplate
{
    private sealed class Data
    {
        public bool HadBlock;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_13.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_13.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        await base.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);

        if (target == base.Owner && props.IsPoweredAttack())
            GetInternalData<Data>().HadBlock = target.Block > 0;
    }

    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != base.Owner || base.Amount <= 0 || amount <= 0m || !props.IsPoweredAttack())
            return amount;
        if (!GetInternalData<Data>().HadBlock)
            return amount;

        return Math.Max(0m, amount - base.Amount);
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
        GetInternalData<Data>().HadBlock = false;
    }
}