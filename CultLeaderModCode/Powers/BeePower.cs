using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 朱bee：敌方回合开始时获得虚弱并受到伤害。
/// </summary>
[RegisterPower]
public class BeePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/bee.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/bee.png";

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);

        if (side != CombatSide.Enemy || base.Owner == null || !participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        Flash();
        var context = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<WeakPower>(context, base.Owner, base.Amount, base.Applier, null);
        await CreatureCmd.Damage(
            context,
            base.Owner,
            5m * base.Amount,
            ValueProp.Unpowered,
            base.Applier ?? base.Owner
        );

        await PowerCmd.Decrement(this);
    }
}
