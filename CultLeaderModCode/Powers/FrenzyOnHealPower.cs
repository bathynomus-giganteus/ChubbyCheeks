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
/// 要来见少女吗？ — 每次恢复生命时，获得1层活力；埃尔德形态下获得狂热。
/// </summary>
[RegisterPower]
public class FrenzyOnHealPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_20.png";

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        await base.AfterCurrentHpChanged(creature, delta);

        if (delta <= 0m || Owner == null || creature != Owner || !creature.IsPlayer || Amount <= 0m)
            return;

        await ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            Amount,
            Owner,
            null
        );
    }
}