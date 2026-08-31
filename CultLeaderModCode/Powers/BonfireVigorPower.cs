using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 篝火 — 回合结束时获得X层活力。埃尔德形态下改为获得等量狂热。
/// </summary>
[RegisterPower]
public class BonfireVigorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_24.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_24.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(Owner) || Amount <= 0) return;
        await ApostlePowerRules.ApplyApostlePower<VigorPower, FervorPower>(
            choiceContext,
            Owner,
            Amount,
            Owner,
            null
        );
    }
}
