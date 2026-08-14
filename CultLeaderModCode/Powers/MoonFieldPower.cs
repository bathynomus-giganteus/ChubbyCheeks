using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Moon Field. At the start of the player's turn, grant temporary Strength equal to this power's stacks.
/// </summary>
[RegisterPower]
public class MoonFieldPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/moon_field.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/moon_field.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        await PowerCmd.Apply<TempStrengthBuffPower>(
            choiceContext,
            base.Owner,
            base.Amount,
            base.Owner,
            null
        );
    }
}