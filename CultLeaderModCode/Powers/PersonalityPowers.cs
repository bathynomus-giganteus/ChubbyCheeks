using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Powers;

// ══════════════════════════════════════════════
//  Personality buffs (Elder-tier) with full effects
// ══════════════════════════════════════════════

public abstract class PersonalityPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}

/// <summary>Life Essence — +5 temp max HP per stack. Uses native AfterPowerAmountChanged for HP sync.</summary>
public sealed class LifeEssencePower : PersonalityPower
{
    private const int HpPerStack = 5;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        int hpDelta = (int)amount * HpPerStack;
        if (hpDelta > 0)
        {
            await CreatureCmd.SetMaxHp(Owner, Owner.MaxHp + hpDelta);
            await CreatureCmd.Heal(Owner, hpDelta);
        }
        else if (hpDelta < 0)
        {
            await CreatureCmd.SetMaxHp(Owner, Math.Max(1, Owner.MaxHp + hpDelta));
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        int amount = Amount;
        if (amount > 0 && Owner.IsAlive)
            await CreatureCmd.SetMaxHp(Owner, Math.Max(1, Owner.MaxHp - amount * HpPerStack));
    }
}

/// <summary>Frozen Fortitude — each stack: +1 Block gained; end of turn: gain 1 Block per stack.</summary>
public sealed class FrozenFortitudePower : PersonalityPower
{
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner)) return;
        int stacks = Amount;
        if (stacks > 0)
            await CreatureCmd.GainBlock(base.Owner, (decimal)stacks, ValueProp.Move, null!, false);
    }
}

/// <summary>Fanaticism — per stack: next Attack +3 damage, consume 1 stack, lose 3 HP.</summary>
public sealed class FanaticismPower : PersonalityPower
{
    private const int DmgBonus = 3;
    private const int HpCost = 3;

    private class Data
    {
        public AttackCommand? commandToModify;
    }

    protected override object? InitInternalData() => new Data();

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != base.Owner || Amount <= 0) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;
        if (command.ModelSource != null && !(command.ModelSource is CardModel)) return Task.CompletedTask;

        var data = GetInternalData<Data>();
        if (data.commandToModify != null) return Task.CompletedTask;
        data.commandToModify = command;
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (base.Owner != dealer || Amount <= 0) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        var data = GetInternalData<Data>();
        if (data.commandToModify != null && cardSource != null && cardSource != data.commandToModify.ModelSource) return 0m;
        return Amount * DmgBonus;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        var data = GetInternalData<Data>();
        if (command != data.commandToModify) return;
        data.commandToModify = null;

        await PowerCmd.ModifyAmount(choiceContext, this, -1m, null, null);
        if (base.Owner.IsAlive)
            await CreatureCmd.Damage(choiceContext, base.Owner, (decimal)HpCost, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner, null, null);
    }
}

/// <summary>Happiness — when stacks >= 3, consume 3, gain 1 Energy + draw 2.</summary>
public sealed class HappinessPower : PersonalityPower
{
    private const int Threshold = 3;
    private bool _triggering;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (_triggering) return;
        if (Amount < Threshold) return;

        _triggering = true;
        if (base.Owner.Player != null)
            await PlayerCmd.GainEnergy(1m, base.Owner.Player);
                await PlayerCmd.GainEnergy(1m, base.Owner.Player);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 2m, base.Owner.Player!, false);
        _triggering = false;
    }
}

// ══════════════════════════════════════════════
//  Base buffs (personality signature powers)
// ══════════════════════════════════════════════

/// <summary>Bitter Pain (苦痛) — per stack at turn end, random debuff to all enemies + self.</summary>
public sealed class BitterPainPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    private static readonly MethodInfo PowerCmdApply6 = typeof(PowerCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 6
            && m.GetParameters()[1].ParameterType == typeof(Creature));

    private static Task ApplyInternal(PlayerChoiceContext ctx, Creature target, decimal amount, Creature applier, string className)
    {
        var type = typeof(PowerModel).Assembly.GetType("MegaCrit.Sts2.Core.Models.Powers." + className)
                   ?? typeof(PowerModel).Assembly.GetType("MegaCrit.Sts2.Core.Models." + className);
        if (type == null) return Task.CompletedTask;
        var apply = PowerCmdApply6.MakeGenericMethod(type);
        return (Task)apply.Invoke(null, [ctx, target, amount, applier, null!, false])!;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner)) return;
        int stacks = Amount;
        if (stacks <= 0) return;

        var combatState = base.Owner.CombatState;
        if (combatState == null) return;
        var allTargets = combatState.Enemies.Concat([base.Owner]).ToList();

        var rng = new Random();
        for (int i = 0; i < stacks; i++)
        {
            int roll = rng.Next(5);
            foreach (var target in allTargets)
            {
                switch (roll)
                {
                    case 0: await PowerCmd.Apply<VulnerablePower>(choiceContext, target, 1m, base.Owner, null!); break;
                    case 1: await ApplyInternal(choiceContext, target, 1m, base.Owner, "WeakPower"); break;
                    case 2: await ApplyInternal(choiceContext, target, 1m, base.Owner, "FrailPower"); break;
                    case 3: await ApplyInternal(choiceContext, target, 3m, base.Owner, "PoisonPower"); break;
                    case 4: await ApplyInternal(choiceContext, target, 6m, base.Owner, "DoomPower"); break;
                }
            }
        }
    }
}

/// <summary>Bitter Pain Burst (苦痛爆发) — end of turn: per stack, all enemies get full debuff set.</summary>
public sealed class BitterPainBurstPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    private static readonly MethodInfo PowerCmdApply6 = typeof(PowerCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 6
            && m.GetParameters()[1].ParameterType == typeof(Creature));

    private static Task ApplyInternal(PlayerChoiceContext ctx, Creature target, decimal amount, Creature applier, string className)
    {
        var type = typeof(PowerModel).Assembly.GetType("MegaCrit.Sts2.Core.Models.Powers." + className)
                   ?? typeof(PowerModel).Assembly.GetType("MegaCrit.Sts2.Core.Models." + className);
        if (type == null) return Task.CompletedTask;
        var apply = PowerCmdApply6.MakeGenericMethod(type);
        return (Task)apply.Invoke(null, [ctx, target, amount, applier, null!, false])!;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner)) return;
        int stacks = Amount;
        if (stacks <= 0) return;

        var combatState = base.Owner.CombatState;
        if (combatState == null) return;
        var enemies = combatState.Enemies.ToList();

        for (int i = 0; i < stacks; i++)
        {
            foreach (var enemy in enemies)
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 1m, base.Owner, null!);
                await ApplyInternal(choiceContext, enemy, 1m, base.Owner, "WeakPower");
                await ApplyInternal(choiceContext, enemy, 1m, base.Owner, "FrailPower");
                await ApplyInternal(choiceContext, enemy, 3m, base.Owner, "PoisonPower");
                await ApplyInternal(choiceContext, enemy, 6m, base.Owner, "DoomPower");
            }
        }
    }
}

// ══════════════════════════════════════════════
//  Marker powers and utilities
// ══════════════════════════════════════════════

public sealed class ElderFormPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
}

/// <summary>Self-stun: end current turn and skip next turn.</summary>
public sealed class SelfStunPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner) && side == CombatSide.Player)
        {
            Flash();
            if (base.Owner.Player != null)
                CombatManager.Instance.SetReadyToEndTurn(base.Owner.Player, false, null);
            await PowerCmd.Remove(this);
        }
    }
}

/// <summary>Track temporary max HP added by LookAtMe card. Removed at combat end.</summary>
public sealed class TempHpTrackerPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        int amount = Amount;
        if (amount > 0 && base.Owner.IsAlive)
            await CreatureCmd.SetMaxHp(base.Owner, Math.Max(1, base.Owner.MaxHp - amount));
    }
}

/// <summary>Plated Armor wrapper — end of player turn: gain Block per stack.</summary>
public sealed class CultPlatedArmorPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner)) return;
        int stacks = Amount;
        if (stacks > 0)
            await CreatureCmd.GainBlock(base.Owner, (decimal)stacks, ValueProp.Move, null!, false);
    }
}