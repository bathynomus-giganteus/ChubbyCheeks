using System;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Life Essence. Each stack grants 5 stacks of TempMaxHpPower (5 max HP).
/// Active trigger heals 5 HP and consumes 1 stack.
/// </summary>
[RegisterPower]
public class LifeEssencePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/lifeessence.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/lifeessence.png";

    internal int TempMaxHpContribution;

    public async Task TriggerActive(PlayerChoiceContext choiceContext, Creature? applier, CardModel? cardSource)
    {
        if (base.Amount <= 0) return;
        await CreatureCmd.Heal(base.Owner, 5m, true);
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, applier, cardSource, silent: true);
    }

    public async Task SyncTempMaxHp(PlayerChoiceContext choiceContext, Creature? applier, CardModel? cardSource)
    {
        if (base.Owner == null)
            return;

        int desired = base.Amount * 5;
        int delta = desired - TempMaxHpContribution;
        if (delta == 0)
            return;

        TempMaxHpContribution = desired;
        var tempHp = base.Owner.Powers?.OfType<TempMaxHpPower>().FirstOrDefault();
        if (tempHp != null)
            await PowerCmd.ModifyAmount(choiceContext, tempHp, delta, applier, cardSource, silent: true);
        else if (delta > 0)
            await PowerCmd.Apply<TempMaxHpPower>(choiceContext, base.Owner, delta, applier, cardSource);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        int delta = -TempMaxHpContribution;
        TempMaxHpContribution = 0;

        if (oldOwner != null && delta != 0)
        {
            var tempHp = oldOwner.Powers?.OfType<TempMaxHpPower>().FirstOrDefault();
            if (tempHp != null)
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), tempHp, delta, null, null, silent: true);
        }

        await base.AfterRemoved(oldOwner);
    }
}