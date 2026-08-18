using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Magic Bullet. Consumed when the Magic Bullet Shooter is played; applies every Bitter Pain Burst debuff to its target.
/// </summary>
[RegisterPower]
public class MagicBulletPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/magic_bullet.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/magic_bullet.png";

    public async Task TriggerMagicBullet(
        PlayerChoiceContext choiceContext,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (base.Amount <= 0 || target == null)
            return;

        await PowerCmd.ModifyAmount(choiceContext, this, -1m, applier, cardSource, silent: true);

        await PowerCmd.Apply<VulnerablePower>(choiceContext, target, 1m, applier, cardSource);
        await PowerCmd.Apply<WeakPower>(choiceContext, target, 1m, applier, cardSource);
        await PowerCmd.Apply<PoisonPower>(choiceContext, target, 2m, applier, cardSource);
        await PowerCmd.Apply<DoomPower>(choiceContext, target, 4m, applier, cardSource);
    }
}
