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
/// 本回合力竭 — 记录临时力量减少，回合结束时恢复。
/// </summary>
[RegisterPower]
public class TempStrengthLossPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomIconPath => "res://CultLeaderMod/images/card_portraits/pure/投降投降了啦.png";
    public override string? CustomBigIconPath => "res://CultLeaderMod/images/card_portraits/pure/投降投降了啦.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.BeforeSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(Owner)) return;
        var recovery = Amount;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, recovery, Owner, null);
        await PowerCmd.Remove<TempStrengthLossPower>(Owner);
    }
}