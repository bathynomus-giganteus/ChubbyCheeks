using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class PastelOutingRelic : CultLeaderModRelic
{
    private int _apostleCardsPlayedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/pastel_outing.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/pastel_outing.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/pastel_outing.png";

    public override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner)
            _apostleCardsPlayedThisTurn = 0;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        var turnNumber = Owner.PlayerCombatState?.TurnNumber;
        if (card.Owner != Owner
            || !ApostlePowerRules.IsApostleCard(card)
            || turnNumber == null
            || _apostleCardsPlayedThisTurn + 1 != turnNumber.Value)
            return false;

        modifiedCost = 0m;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == Owner
            && cardPlay.IsFirstInSeries
            && ApostlePowerRules.IsApostleCard(cardPlay.Card))
            _apostleCardsPlayedThisTurn++;

        return Task.CompletedTask;
    }
}
