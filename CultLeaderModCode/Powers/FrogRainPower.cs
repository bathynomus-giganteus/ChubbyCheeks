using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Frog Rain. At the start of the player's turn, grant Retain stacks and heal 1 HP.
/// </summary>
[RegisterPower]
public class FrogRainPower : ModPowerTemplate
{
    private sealed class Data
    {
        public decimal RetainPerTurn = 2m;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/frog_rain.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/frog_rain.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void ConfigureRetainPerTurn(decimal amount)
    {
        GetInternalData<Data>().RetainPerTurn = amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            base.Owner,
            GetInternalData<Data>().RetainPerTurn,
            base.Owner,
            null
        );
        await CreatureCmd.Heal(base.Owner, 1m);
        await PowerCmd.Decrement(this);
    }
}
