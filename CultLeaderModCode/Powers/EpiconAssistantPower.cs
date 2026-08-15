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
/// Epicon Assistant. At the start of the player's turn, grant one stack of each base apostle buff.
/// </summary>
[RegisterPower]
public class EpiconAssistantPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/card_portraits/lively/epicon_assistant.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/card_portraits/lively/epicon_assistant.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0)
            return;

        await ApostleCardPlayHelpers.ApplyPurePower(choiceContext, base.Owner, base.Amount, base.Owner, null);
        await ApostleCardPlayHelpers.ApplyMelancholyPower(choiceContext, base.Owner, base.Amount, base.Owner, null);
        await ApostleCardPlayHelpers.ApplyFrenzyPower(choiceContext, base.Owner, base.Amount, base.Owner, null);
        await ApostleCardPlayHelpers.ApplyCalmPower(choiceContext, base.Owner, base.Amount, base.Owner, null);
    }
}
