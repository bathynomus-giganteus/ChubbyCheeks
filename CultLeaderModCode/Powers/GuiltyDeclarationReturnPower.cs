using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Hidden helper for ????. At the start of each player turn, moves any
/// copies of the card from the exhaust pile back to the draw pile.
/// </summary>
[RegisterPower]
public class GuiltyDeclarationReturnPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
    public override bool ShouldPlayVfx => false;

    public static async Task EnsureTracker(
        PlayerChoiceContext choiceContext,
        Creature owner,
        Creature? applier,
        CardModel? cardSource)
    {
        if (owner == null || owner.GetPower<GuiltyDeclarationReturnPower>() != null)
            return;

        await PowerCmd.Apply<GuiltyDeclarationReturnPower>(
            choiceContext,
            owner,
            1m,
            applier ?? owner,
            cardSource,
            silent: true
        );
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner)
            return;

        var exhaustPile = PileType.Exhaust.GetPile(player);
        var guiltyCards = exhaustPile.Cards.OfType<Apostle_Melancholy_13>().ToList();
        if (guiltyCards.Count == 0)
            return;

        foreach (var card in guiltyCards)
        {
            await CardPileCmd.Add(
                card,
                PileType.Draw,
                CardPilePosition.Top,
                this,
                false
            );
        }
    }
}
