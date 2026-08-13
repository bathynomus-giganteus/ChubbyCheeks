using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 小小塞巴斯蒂安：每有5层，玩家回合结束时获得1点活力（埃尔德形态下为狂热），
/// 并对随机敌人造成1点伤害（伤害与活力均一次性结算）。
/// 受到未被格挡的攻击伤害时，优先消耗塞巴斯蒂安的层数吸收伤害，之后才扣生命值。
/// </summary>
[RegisterPower]
public class SebastianPower : ModPowerTemplate
{
    private sealed class Data
    {
        public decimal PendingAbsorb;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/card_portraits/frenzy/小小塞巴斯蒂安.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/card_portraits/frenzy/小小塞巴斯蒂安.png";

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side != CombatSide.Player || Owner == null || !participants.Contains(Owner) || Amount <= 0)
            return;

        int ticks = (int)(Amount / 5m);
        if (ticks <= 0)
            return;

        await ApostleCardPlayHelpers.ApplyFrenzyPower(
            choiceContext,
            Owner,
            ticks,
            Owner,
            null
        );

        var enemies = Owner.CombatState
            ?.GetCreaturesOnSide(CombatSide.Enemy)
            .Where(enemy => !enemy.IsDead)
            .ToList();

        if (enemies != null && enemies.Count > 0)
        {
            var target = enemies[Random.Shared.Next(enemies.Count)];
            await CreatureCmd.Damage(
                choiceContext,
                target,
                ticks,
                ValueProp.Unpowered,
                Owner,
                null,
                null
            );
        }
    }

    public override decimal ModifyHpLostAfterOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        var result = base.ModifyHpLostAfterOsty(target, amount, props, dealer, cardSource);
        var data = GetInternalData<Data>();
        data.PendingAbsorb = 0m;

        if (Owner == null || target != Owner || amount <= 0m || !props.IsPoweredAttack())
            return result;

        decimal stacks = Amount;
        if (stacks <= 0m)
            return result;

        decimal absorbed = Math.Min(amount, stacks);
        data.PendingAbsorb = absorbed;
        return amount - absorbed;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        var data = GetInternalData<Data>();
        if (data.PendingAbsorb > 0m)
        {
            decimal absorb = data.PendingAbsorb;
            data.PendingAbsorb = 0m;
            Flash();
            await PowerCmd.ModifyAmount(
                new ThrowingPlayerChoiceContext(),
                this,
                -absorb,
                null,
                null,
                silent: true
            );
        }

        await base.AfterModifyingHpLostAfterOsty();
    }
}