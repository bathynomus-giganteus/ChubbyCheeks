using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using System;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 生命本源 — 每层+5临时最大HP。HP变更由 PowerInterceptPatch 的 Harmony Postfix 驱动。
/// 主动触发时回复5HP并消耗1层。战斗结束自动清理。
/// </summary>
[RegisterPower]
public class LifeEssencePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/lifeessence.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/lifeessence.png";

    internal int GrantedHp;
    internal int TrackedAmount;

    public async Task TriggerActive(PlayerChoiceContext choiceContext, Creature? applier, CardModel? cardSource)
    {
        if (base.Amount <= 0) return;
        await CreatureCmd.Heal(base.Owner, 5m, true);
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, applier, cardSource, silent: true);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (GrantedHp > 0 && oldOwner != null)
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), oldOwner, GrantedHp, false);
        GrantedHp = 0;
        TrackedAmount = 0;
        await base.AfterRemoved(oldOwner);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (GrantedHp > 0)
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner, GrantedHp, false);
            GrantedHp = 0;
            TrackedAmount = 0;
        }
        await base.AfterCombatEnd(room);
    }
}