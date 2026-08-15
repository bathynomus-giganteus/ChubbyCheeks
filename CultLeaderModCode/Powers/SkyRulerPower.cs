using System.Linq;
using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// Sky Ruler. While this buff is active, grant every enemy one stack of Debilitate
/// (??) at the start of the player's turn, doubling Vulnerable/Weak effects on them.
/// </summary>
[RegisterPower]
public class SkyRulerPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_20.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/忧郁_20.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Owner == null)
            return;

        var enemies = ApostleCardEffectHelpers.AliveEnemies(base.Owner)
            .Where(enemy => enemy.GetPower<DebilitatePower>() == null)
            .ToList();
        if (enemies.Count == 0)
            return;

        await PowerCmd.Apply<DebilitatePower>(
            choiceContext,
            enemies,
            1m,
            base.Owner,
            null
        );
    }
}
