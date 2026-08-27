using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class BitterPainPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/bitterpain.png";
    public override string CustomBigIconPath =>
        "res://CultLeaderMod/images/powers/big/bitterpain.png";

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

        var rng = new Random();
        for (int i = 0; i < base.Amount; i++)
        {
            var enemy = enemies[rng.Next(enemies.Count)];
            switch (rng.Next(4))
            {
                case 0:
                    await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1m, base.Owner, null);
                    break;
                case 1:
                    await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 1m, base.Owner, null);
                    break;
                case 2:
                    await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, 3m, base.Owner, null);
                    break;
                default:
                    await PowerCmd.Apply<DoomPower>(choiceContext, enemy, 6m, base.Owner, null);
                    break;
            }
        }

        await PowerCmd.Decrement(this);
    }
}
