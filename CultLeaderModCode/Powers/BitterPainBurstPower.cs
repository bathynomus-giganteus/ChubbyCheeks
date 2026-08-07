using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 苦痛爆发 — 回合结束时每有一层，所有敌人获得1易伤、1虚弱、1脆弱、3中毒、6灾厄。
/// （不给自身上debuff了）
/// </summary>
[RegisterPower]
public class BitterPainBurstPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/bitterpainburst.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/bitterpainburst.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(base.Owner) || base.Amount <= 0) return;

        var enemies = base.Owner.CombatState.GetCreaturesOnSide(CombatSide.Enemy)
            .Where(c => !c.IsDead).ToList();

        for (int i = 0; i < base.Amount; i++)
        {
            foreach (var enemy in enemies)
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1m, base.Owner, null);
                await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 1m, base.Owner, null);
                await PowerCmd.Apply<FrailPower>(choiceContext, enemy, 1m, base.Owner, null);
                await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, 3m, base.Owner, null);
                await PowerCmd.Apply<DoomPower>(choiceContext, enemy, 6m, base.Owner, null);
            }
        }
    }
}
