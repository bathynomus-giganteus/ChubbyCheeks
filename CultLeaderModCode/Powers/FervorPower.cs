using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 狂热 — 每层使下一张攻击牌伤害+3；攻击结算后只消耗1层，并失去3生命。
/// </summary>
[RegisterPower]
public class FervorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/fervor.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/fervor.png";

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (dealer == base.Owner && cardSource?.Type == CardType.Attack && cardPlay != null && base.Amount > 0)
            return base.Amount * 3m;

        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);
        if (base.Amount <= 0 || command.Attacker != base.Owner || command.CardPlay?.Card.Type != CardType.Attack)
            return;

        await CreatureCmd.Damage(choiceContext, base.Owner, 3m, ValueProp.Unblockable, base.Owner);
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, base.Owner, command.CardPlay.Card, silent: true);
    }
}
