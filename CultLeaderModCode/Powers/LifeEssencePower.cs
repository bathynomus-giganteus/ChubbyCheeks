using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class LifeEssencePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/lifeessence.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/lifeessence.png";

    private int _trackedAmount;
    private bool _initialized;

    public override async Task AfterApplied(Creature applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        if (!_initialized)
        {
            _trackedAmount = base.Amount;
            _initialized = true;
            if (_trackedAmount > 0)
                await CreatureCmd.GainMaxHp(base.Owner, _trackedAmount * 5);
        }
        else
        {
            int delta = base.Amount - _trackedAmount;
            if (delta > 0)
                await CreatureCmd.GainMaxHp(base.Owner, delta * 5);
            _trackedAmount = base.Amount;
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (_trackedAmount > 0 && oldOwner != null)
            await CreatureCmd.LoseMaxHp(null!, oldOwner, _trackedAmount * 5, false);
        await base.AfterRemoved(oldOwner);
    }
}

