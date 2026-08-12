using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[RegisterPower]
public class HappinessPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/happiness.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/happiness.png";

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        if (base.Amount < 3)
            return;

        var player = GetOwnerPlayer();
        if (player == null)
            return;

        var choiceContext = new ThrowingPlayerChoiceContext();
        while (base.Amount >= 3)
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -3m, applier, cardSource, silent: true);
            await CardPileCmd.Draw(choiceContext, 2m, player);
            await PlayerCmd.GainEnergy(1m, player);
        }
    }

    private Player? GetOwnerPlayer()
    {
        var combatState = base.Owner.CombatState;
        if (combatState == null)
            return null;

        return combatState.Players.FirstOrDefault(player => player.Creature == base.Owner);
    }
}
