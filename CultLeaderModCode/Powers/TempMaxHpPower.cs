using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Temporary Max HP buff. Each stack increases max HP by 1 while present.
/// When the power is removed or combat ends, the granted max HP is returned.
/// </summary>
[RegisterPower]
public class TempMaxHpPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/maxHP.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/maxHP.png";

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (!ReferenceEquals(power, this) || amount == 0m || Owner == null)
            return;

        if (amount > 0m)
            await CreatureCmd.GainMaxHp(Owner, amount);
        else
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner, -amount, false);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner != null && Amount > 0)
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), oldOwner, Amount, false);
        await base.AfterRemoved(oldOwner);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner != null && Amount > 0)
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner, Amount, false);
            SetAmount(0, silent: true);
        }
        await base.AfterCombatEnd(room);
    }
}