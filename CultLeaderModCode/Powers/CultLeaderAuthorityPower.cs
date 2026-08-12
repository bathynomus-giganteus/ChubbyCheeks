using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 教主的权能：使徒牌获得对应 buff 时，额外获得等同于权能层数的层数。
/// 权能达到 5 层时，消耗 5 层进入埃尔德形态。
/// </summary>
[RegisterPower]
public class CultLeaderAuthorityPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath =>
        "res://CultLeaderMod/images/powers/cultleaderauthority.png";
    public override string CustomBigIconPath =>
        "res://CultLeaderMod/images/powers/big/cultleaderauthority.png";

    public override decimal ModifyPowerAmountGivenAdditive(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource
    )
    {
        if (ApostlePowerRules.IsConverting || amount <= 0m)
            return 0m;

        if (
            giver != Owner
            || !ApostlePowerRules.IsApostleCard(cardSource)
            || !ApostlePowerRules.IsAuthorityScaledPower(power)
        )
            return 0m;

        return Amount;
    }
}
