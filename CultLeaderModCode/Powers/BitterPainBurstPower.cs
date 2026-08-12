using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 苦痛爆发 — 回合结束时每有一层，所有敌人获得1易伤、1虚弱、3中毒、6灾厄。
/// </summary>
[RegisterPower]
public class BitterPainBurstPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/pain.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/pain.png";

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(base.Owner) || base.Amount <= 0)
            return;

        var enemies = base.Owner.CombatState
            ?.GetCreaturesOnSide(CombatSide.Enemy)
            .Where(c => !c.IsDead)
            .ToList();
        if (enemies == null || enemies.Count == 0)
            return;

        for (int i = 0; i < base.Amount; i++)
        {
            foreach (var enemy in enemies)
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1m, base.Owner, null);
                await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 1m, base.Owner, null);
                await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, 3m, base.Owner, null);
                await PowerCmd.Apply<DoomPower>(choiceContext, enemy, 6m, base.Owner, null);
            }
        }
    }
}
