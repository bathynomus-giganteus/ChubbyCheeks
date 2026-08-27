using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Extant. At the end of the player's turn, for each card retained this turn,
/// enemies with Extant take 3 damage per Extant stack.
/// </summary>
[RegisterPower]
public class ExtantPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/active.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/active.png";

    private const decimal DamagePerRetainedCard = 3m;

    public override async Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        await base.AfterFlush(choiceContext, player, flushedCards, retainedCards);

        if (base.Owner == null || base.Owner.IsDead || base.Owner.IsPlayer || base.Amount <= 0m)
            return;
        if (player?.Creature == null || retainedCards == null || retainedCards.Count == 0)
            return;

        decimal damage = DamagePerRetainedCard * retainedCards.Count * base.Amount;
        if (damage <= 0m)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            base.Owner,
            damage,
            ValueProp.Unpowered,
            player.Creature,
            null,
            null
        );
    }
}
