using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>Whenever you heal, deal damage to a random enemy. (Carrot - Sap Pump)</summary>
public sealed class HealDamagePower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterHeal(PlayerChoiceContext ctx, decimal amount)
    {
        var enemies = base.Owner.CombatState?.Enemies.Where(e => !e.IsDead).ToList();
        if (enemies == null || enemies.Count == 0) return;
        var target = enemies[Random.Shared.Next(enemies.Count)];
        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, (decimal)Amount, ValueProp.Unpowered, base.Owner);
    }
}

/// <summary>Countdown: every N heals, gain 1 energy and reset. (Laika - Remote Charging)</summary>
public sealed class HealEnergyPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    private int _threshold;

    public int Threshold
    {
        get => _threshold > 0 ? _threshold : (int)Amount;
        set => _threshold = value;
    }

    public override async Task AfterHeal(PlayerChoiceContext ctx, decimal amount)
    {
        if (_threshold <= 0) _threshold = (int)Amount;
        int current = (int)Amount;
        if (current <= 1)
        {
            await PowerCmd.ModifyAmount(ctx, this, (decimal)(_threshold - current), base.Owner, null, false);
            Flash();
            if (base.Owner.Player != null)
                await PlayerCmd.GainEnergy(1m, base.Owner.Player);
        }
        else
        {
            await PowerCmd.ModifyAmount(ctx, this, -1m, base.Owner, null, false);
        }
    }
}

/// <summary>Debuff on enemy: whenever the player heals, take multiplied damage. (Mute - Basic Hack, old design - no longer used)</summary>
public sealed class HackIntrusionPower : CultLeaderModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public int Multiplier => (int)Amount;
}

/// <summary>Patch CreatureCmd.Heal to call AfterHeal on all CultLeaderModPowers on the healed creature.</summary>
[HarmonyPatch]
public static class HealTriggerPatch
{
    private static MethodBase TargetMethod() =>
        typeof(CreatureCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Heal" && m.GetParameters().Length == 3
                && m.GetParameters()[0].ParameterType == typeof(Creature));

    [HarmonyPostfix]
    private static void Postfix(Creature creature, decimal amount, bool playAnim, ref Task __result)
    {
        __result = AfterHealHookAsync(__result, creature, amount);
    }

    private static async Task AfterHealHookAsync(Task healTask, Creature creature, decimal amount)
    {
        await healTask;
        if (amount <= 0 || creature.CombatState == null) return;

        try
        {
            var ctx = new ThrowingPlayerChoiceContext();

            foreach (var power in creature.Powers.ToList())
            {
                if (power is CultLeaderModPower clPower)
                    await clPower.AfterHeal(ctx, amount);
            }

            foreach (var enemy in creature.CombatState.Enemies.Where(e => !e.IsDead).ToList())
            {
                var hack = enemy.GetPower<HackIntrusionPower>();
                if (hack != null)
                    await CreatureCmd.Damage(ctx, enemy, (decimal)hack.Multiplier * amount, ValueProp.Unpowered, creature);
            }
        }
        catch (Exception ex)
        {
            Godot.GD.PrintErr($"[CultLeaderMod] AfterHealHook error: {ex}");
        }
    }
}