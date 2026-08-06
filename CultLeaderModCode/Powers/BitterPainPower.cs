using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    public override string CustomIconPath => "CultLeaderMod/images/powers/bitterpain.png";
    public override string CustomBigIconPath => "CultLeaderMod/images/powers/big/bitterpain.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(base.Owner) || base.Amount <= 0) return;

        var enemies = base.Owner.CombatState.GetCreaturesOnSide(CombatSide.Enemy)
            .Where(c => !c.IsDead).ToList();
        var rng = new Random();

        for (int i = 0; i < base.Amount; i++)
        {
            int idx = rng.Next(5);
            foreach (var enemy in enemies)
            {
                switch (idx)
                {
                    case 0: await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1m, base.Owner, null); break;
                    case 1: await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 1m, base.Owner, null); break;
                    case 2: await PowerCmd.Apply<FrailPower>(choiceContext, enemy, 1m, base.Owner, null); break;
                    case 3: await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, 3m, base.Owner, null); break;
                    case 4: await PowerCmd.Apply<DoomPower>(choiceContext, enemy, 6m, base.Owner, null); break;
                }
            }
            switch (idx)
            {
                case 0: await PowerCmd.Apply<VulnerablePower>(choiceContext, base.Owner, 1m, base.Owner, null); break;
                case 1: await PowerCmd.Apply<WeakPower>(choiceContext, base.Owner, 1m, base.Owner, null); break;
                case 2: await PowerCmd.Apply<FrailPower>(choiceContext, base.Owner, 1m, base.Owner, null); break;
                case 3: await PowerCmd.Apply<PoisonPower>(choiceContext, base.Owner, 3m, base.Owner, null); break;
                case 4: await PowerCmd.Apply<DoomPower>(choiceContext, base.Owner, 6m, base.Owner, null); break;
            }
        }
    }
}
