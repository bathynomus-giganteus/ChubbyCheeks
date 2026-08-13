using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 汁液泵机发射 — 每次恢复生命时，对随机敌人造成等同于层数(Amount)的伤害。
/// </summary>
[RegisterPower]
public class SapLauncherPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_卡罗特.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_卡罗特.png";

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        await base.AfterCurrentHpChanged(creature, delta);
        if (delta <= 0 || creature != base.Owner || base.Amount <= 0)
            return;

        var target = ApostleCardEffectHelpers.RandomEnemy(base.Owner);
        if (target == null)
            return;

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            target,
            base.Amount,
            ValueProp.Unpowered,
            base.Owner,
            null,
            null
        );
    }
}