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

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (power != this || amount <= 0m)
            return;

        var player = GetOwnerPlayer();
        if (player == null)
            return;

        int previousTriggers = (int)Math.Floor((base.Amount - amount) / 3m);
        int currentTriggers = (int)Math.Floor(base.Amount / 3m);
        int triggerCount = Math.Max(0, currentTriggers - previousTriggers);

        for (int i = 0; i < triggerCount; i++)
        {
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
