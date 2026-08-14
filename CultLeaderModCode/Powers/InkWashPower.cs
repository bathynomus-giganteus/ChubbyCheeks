using MegaCrit.Sts2.Core.Commands;
using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Ink Wash. At the end of the current turn, gain Plating equal to stacks multiplied by the number of cards in hand.
/// </summary>
[RegisterPower]
public class InkWashPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_19.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/冷静_19.png";

    public override async Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
    {
        await base.BeforeFlush(choiceContext, player);

        if (player.Creature != base.Owner || base.Owner == null)
            return;

        int handCount = PileType.Hand.GetPile(player).Cards.Count;
        int stacks = Math.Max(0, (int)base.Amount);
        int plating = stacks * handCount;
        if (plating > 0)
        {
            await ApostleCardPlayHelpers.ApplyCalmPower(
                choiceContext,
                base.Owner,
                plating,
                base.Owner,
                null
            );
        }

        await PowerCmd.Remove(this);
    }
}
