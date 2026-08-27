using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 开核桃大师：回合结束时，在覆甲等回合结束格挡结算后，按当前格挡的一半获得保留。
/// </summary>
[RegisterPower]
public class WalnutMasterPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_21.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_21.png";

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side != CombatSide.Player || base.Owner == null || !participants.Contains(base.Owner) || base.Amount <= 0m)
            return;

        decimal retain = Math.Floor(base.Owner.Block / 2m);
        if (retain > 0m)
        {
            Flash();
            await ApostleCardPlayHelpers.ApplyLivelyPower(
                choiceContext,
                base.Owner,
                retain,
                base.Owner,
                null
            );
        }

        await PowerCmd.Decrement(this);
    }
}
