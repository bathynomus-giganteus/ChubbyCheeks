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
/// 治愈。受到攻击伤害时，消耗等同于本次伤害的层数；每消耗1层回复1点生命。
/// 回合结束时不会自动触发。
/// </summary>
[RegisterPower]
public class HealingPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/heal.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/heal.png";

    public async Task TriggerActive(PlayerChoiceContext choiceContext, Creature? applier, CardModel? cardSource)
    {
        if (base.Amount <= 0) return;

        await CreatureCmd.Heal(base.Owner, 1m, true);
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, applier, cardSource, silent: true);
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

        if (target != base.Owner || base.Amount <= 0 || result.UnblockedDamage <= 0 || !props.IsPoweredAttack())
            return;

        decimal trigger = Math.Min(base.Amount, result.UnblockedDamage);
        if (trigger <= 0m)
            return;

        await CreatureCmd.Heal(base.Owner, trigger, true);
        await PowerCmd.ModifyAmount(choiceContext, this, -trigger, base.Applier ?? dealer, cardSource, silent: true);
    }
}
