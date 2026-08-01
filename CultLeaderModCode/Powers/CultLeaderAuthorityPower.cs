using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Powers;

public sealed class CultLeaderAuthorityPower : CultLeaderModPower
{
    public const int MaxStacks = 5;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override bool TryModifyPowerAmountReceived(
        PowerModel power,
        Creature target,
        decimal amount,
        Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;

        if (power is not CultLeaderAuthorityPower)
            return false;

        var currentAmount = target.GetPowerAmount<CultLeaderAuthorityPower>();
        modifiedAmount = Math.Clamp(amount, -currentAmount, MaxStacks - currentAmount);
        return modifiedAmount != amount;
    }
}
