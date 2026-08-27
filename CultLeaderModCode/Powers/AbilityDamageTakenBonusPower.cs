using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 围猎标记：玩家通过能力牌对该敌人造成的伤害按 Amount% 提升。
/// </summary>
[RegisterPower]
public class AbilityDamageTakenBonusPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_岚.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_岚.png";

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (
            target == base.Owner
            && !base.Owner.IsPlayer
            && dealer?.IsPlayer == true
            && cardSource?.Type == CardType.Power
            && base.Amount > 0m
        )
            return amount * base.Amount / 100m;

        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
    }
}
