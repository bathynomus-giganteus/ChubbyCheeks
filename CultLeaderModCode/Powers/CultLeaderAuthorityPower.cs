using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Powers;

public sealed class CultLeaderAuthorityPower : CultLeaderModPower
{
    public const int MaxStacks = 5;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    /// <summary>Clamp incoming Authority stacks to 0..MaxStacks.</summary>
    public override bool TryModifyPowerAmountReceived(
        PowerModel power,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (power is not CultLeaderAuthorityPower) return false;

        var current = target.GetPowerAmount<CultLeaderAuthorityPower>();
        modifiedAmount = Math.Clamp(amount, -current, MaxStacks - current);
        return modifiedAmount != amount;
    }

    /// <summary>When Authority reaches 5, consume 5 stacks and grant Elder Form.</summary>
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Amount < MaxStacks) return;
        if (base.Owner.GetPowerAmount<ElderFormPower>() > 0) return;

        var ctx = new BlockingPlayerChoiceContext();
        await PowerCmd.ModifyAmount(ctx, this, -MaxStacks, applier, cardSource);
        await PowerCmd.Apply<ElderFormPower>(ctx, base.Owner, 1m, applier, cardSource);
        await ElderFormHelper.ConvertBaseBuffsToElder(ctx, base.Owner);
    }
}