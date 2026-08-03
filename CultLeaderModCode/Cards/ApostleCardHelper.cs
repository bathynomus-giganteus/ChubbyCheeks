using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Cards;

/// <summary>Shared helpers for apostle cards that apply powers with Elder Form switching.</summary>
public static class ApostleCardHelper
{
    private const int LifeEssenceHpPerStack = 5;

    private static readonly MethodInfo PowerCmdApplyTyped = typeof(PowerCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 6
            && m.GetParameters()[1].ParameterType == typeof(Creature));

    /// <summary>Get the base power type for a personality (Regen, Plating, Vigor, BitterPain, Artifact).</summary>
    public static Type BasePowerTypeFor(ApostlePersonality p) => ApostlePersonalityMap.BasePowerType(p);

    /// <summary>Get the elder power type for a personality (LifeEssence, FrozenFortitude, Fanaticism, etc.).</summary>
    public static Type ElderPowerTypeFor(ApostlePersonality p) => ApostlePersonalityMap.ElderPowerType(p);

    /// <summary>Apply base buff or elder buff depending on Elder Form state, per personality.</summary>
    public static Task ApplyBaseOrElder(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        Creature applier, CardModel source, bool hasElderForm, ApostlePersonality personality)
    {
        Type effectiveType = hasElderForm
            ? ElderPowerTypeFor(personality)
            : BasePowerTypeFor(personality);
        var typedApply = PowerCmdApplyTyped.MakeGenericMethod(effectiveType);
        return (Task)typedApply.Invoke(null, [ctx, target, amount, applier, source, false])!;
    }

    /// <summary>Apply buff + authority bonus + elder-form side effects.</summary>
    public static async Task ApplyWithAuthority(
        PlayerChoiceContext ctx, Creature owner, decimal baseAmount,
        CardModel source, bool hasElderForm, ApostlePersonality personality)
    {
        await ApplyBaseOrElder(ctx, owner, baseAmount, owner, source, hasElderForm, personality);
        int authority = (int)owner.GetPowerAmount<Powers.CultLeaderAuthorityPower>();
        if (authority > 0)
            await ApplyBaseOrElder(ctx, owner, authority, owner, source, hasElderForm, personality);
        if (hasElderForm && personality == ApostlePersonality.Pure)
            await SyncLifeEssenceHp(ctx, owner);
    }

    public static async Task SyncLifeEssenceHp(PlayerChoiceContext ctx, Creature owner)
    {
        var le = owner.GetPower<Powers.LifeEssencePower>();
        int desired = le != null ? (int)le.Amount * LifeEssenceHpPerStack : 0;
        var tracker = owner.GetPower<Powers.TempHpTrackerPower>();
        int current = tracker != null ? (int)tracker.Amount : 0;
        int delta = desired - current;
        if (delta == 0) return;

        await CreatureCmd.SetMaxHp(owner, Math.Max(1, owner.MaxHp + delta));
        if (delta > 0)
            await CreatureCmd.Heal(owner, delta);

        if (tracker != null)
        {
            if (desired == 0)
                await PowerCmd.Remove(tracker);
            else
                await PowerCmd.ModifyAmount(ctx, tracker, delta, owner, null, false);
        }
        else if (desired > 0)
        {
            await PowerCmd.Apply<Powers.TempHpTrackerPower>(ctx, owner, desired, owner, null, false);
        }
    }

    public static async Task TriggerRegenOrLifeEssence(
        PlayerChoiceContext ctx, Creature owner, int times, CardModel? source = null)
    {
        bool hasElderForm = owner.GetPowerAmount<Powers.ElderFormPower>() > 0;
        if (hasElderForm)
        {
            var le = owner.GetPower<Powers.LifeEssencePower>();
            if (le != null)
            {
                for (int i = 0; i < times; i++)
                {
                    if (le.Amount <= 0) break;
                    await CreatureCmd.Heal(owner, 5m);
                    await PowerCmd.Decrement(le);
                }
                await SyncLifeEssenceHp(ctx, owner);
            }
        }
        else
        {
            for (int i = 0; i < times; i++)
            {
                var regen = owner.GetPower<MegaCrit.Sts2.Core.Models.Powers.RegenPower>();
                if (regen == null || regen.Amount <= 0) break;
                await CreatureCmd.Heal(owner, regen.Amount);
                await PowerCmd.Decrement(regen);
            }
        }
    }

    public static int TotalRegenStacks(Creature creature) =>
        (int)(creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.RegenPower>()
            + creature.GetPowerAmount<Powers.LifeEssencePower>());

    public static Task ApplyPower<T>(PlayerChoiceContext ctx, Creature target,
        decimal amount, Creature applier, CardModel source)
        where T : PowerModel
    {
        return PowerCmd.Apply<T>(ctx, target, amount, applier, source);
    }
}