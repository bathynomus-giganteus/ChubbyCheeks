using System.Threading.Tasks;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Relics;

[RegisterRelic(typeof(CultLeaderModRelicPool))]
public class ClearWeatherCardRelic : CultLeaderModRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override string? CustomIconPath => "res://CultLeaderMod/images/relics/clear_weather_card.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/relics/clear_weather_card.png";
    public override string? CustomIconOutlinePath => "res://CultLeaderMod/images/relics/clear_weather_card.png";

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner)
            return;

        var playerCombatState = base.Owner.PlayerCombatState;
        if (playerCombatState == null)
            return;

        int turnNumber = playerCombatState.TurnNumber;
        if (turnNumber <= 1)
        {
            Flash();
            await PowerCmd.Apply<CultLeaderAuthorityPower>(
                choiceContext,
                base.Owner.Creature,
                1m,
                base.Owner.Creature,
                null
            );
        }
        else if (turnNumber == 4)
        {
            var authority = base.Owner.Creature.GetPower<CultLeaderAuthorityPower>();
            if (authority == null || authority.Amount <= 0m)
                return;

            Flash();
            await PowerCmd.ModifyAmount(
                choiceContext,
                authority,
                -1m,
                base.Owner.Creature,
                null
            );
        }
    }
}
