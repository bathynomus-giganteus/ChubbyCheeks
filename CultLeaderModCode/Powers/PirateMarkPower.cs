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
/// Pirate Mark. Whenever a player damages this enemy, the player gains gold and Healing equal to this power's stacks.
/// </summary>
[RegisterPower]
public class PirateMarkPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/pirate_mark.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/pirate_mark.png";

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (target != base.Owner || base.Amount <= 0 || result.UnblockedDamage <= 0 || dealer == null || !dealer.IsPlayer)
            return;

        decimal reward = base.Amount;
        await PlayerCmd.GainGold(reward, dealer.Player!, false);
        await PowerCmd.Apply<HealingPower>(choiceContext, dealer, reward, base.Owner, cardSource);
    }
}
