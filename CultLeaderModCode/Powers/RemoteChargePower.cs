using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 远程充电 — 每次恢复生命消耗1层，层数扣到0时获得1点能量并重置为阈值。
/// </summary>
[RegisterPower]
public class RemoteChargePower : ModPowerTemplate
{
    private int _threshold;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_莱卡.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_莱卡.png";

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _threshold = (int)base.Amount;
        return base.AfterApplied(applier, cardSource);
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        await base.AfterCurrentHpChanged(creature, delta);
        if (delta <= 0 || creature != base.Owner || base.Amount <= 0)
            return;

        if (_threshold <= 0)
            _threshold = (int)base.Amount;

        if (base.Amount <= 1m)
        {
            var player = base.Owner.Player;
            if (player != null)
                await PlayerCmd.GainEnergy(1m, player);
            await PowerCmd.ModifyAmount(
                new ThrowingPlayerChoiceContext(),
                this,
                _threshold - base.Amount,
                base.Applier,
                null,
                silent: true
            );
        }
        else
        {
            await PowerCmd.ModifyAmount(
                new ThrowingPlayerChoiceContext(),
                this,
                -1m,
                base.Applier,
                null,
                silent: true
            );
        }
    }
}