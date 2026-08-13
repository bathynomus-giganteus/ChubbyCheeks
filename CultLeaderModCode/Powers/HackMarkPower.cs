using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Basic Hacking Attack debuff. Whenever the player heals, the owner of this
/// debuff takes damage equal to the healed amount multiplied by its stacks.
/// </summary>
[RegisterPower]
public class HackMarkPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/card_portraits/pure/基础黑客攻击.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/card_portraits/pure/基础黑客攻击.png";

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        await base.AfterCurrentHpChanged(creature, delta);

        if (delta <= 0m || !creature.IsPlayer || Owner == null || Owner.IsDead || Amount <= 0)
            return;

        var damage = delta * Amount;
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            Owner,
            damage,
            ValueProp.Unpowered,
            creature,
            null,
            null
        );
    }
}