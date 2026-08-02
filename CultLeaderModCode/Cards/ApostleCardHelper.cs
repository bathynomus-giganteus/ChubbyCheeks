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
    private static readonly MethodInfo PowerCmdApplyTyped = typeof(PowerCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition
            && m.GetParameters().Length == 6
            && m.GetParameters()[1].ParameterType == typeof(Creature));

    /// <summary>Apply Regen or LifeEssence depending on ElderForm state.</summary>
    public static Task ApplyRegenOrLifeEssence(
        PlayerChoiceContext ctx,
        Creature target,
        decimal amount,
        Creature applier,
        CardModel source,
        bool hasElderForm)
    {
        Type effectiveType = hasElderForm ? typeof(Powers.LifeEssencePower) : typeof(MegaCrit.Sts2.Core.Models.Powers.RegenPower);
        var typedApply = PowerCmdApplyTyped.MakeGenericMethod(effectiveType);
        return (Task)typedApply.Invoke(null, [ctx, target, amount, applier, source, false])!;
    }

    /// <summary>
    /// Apply Regen/LifeEssence with base amount + extra stacks from CultLeaderAuthority.
    /// </summary>
    public static async Task ApplyWithAuthority(
        PlayerChoiceContext ctx,
        Creature owner,
        decimal baseAmount,
        CardModel source,
        bool hasElderForm)
    {
        await ApplyRegenOrLifeEssence(ctx, owner, baseAmount, owner, source, hasElderForm);
        int authority = (int)owner.GetPowerAmount<Powers.CultLeaderAuthorityPower>();
        if (authority > 0)
            await ApplyRegenOrLifeEssence(ctx, owner, authority, owner, source, hasElderForm);
    }

    /// <summary>
    /// Trigger Regen (or LifeEssence if ElderForm) N times.
    /// Regen trigger: heal full Amount + Decrement.
    /// LifeEssence: heal 5 per trigger + raise max HP by 5 + Decrement.
    /// </summary>
    public static async Task TriggerRegenOrLifeEssence(PlayerChoiceContext ctx, Creature owner, int times, CardModel? source = null)
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
                    await CreatureCmd.SetMaxHp(owner, owner.MaxHp + 5);
                    await CreatureCmd.Heal(owner, 5m);
                    await PowerCmd.Decrement(le);
                }
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

    /// <summary>Get total Regen + LifeEssence stacks on a creature.</summary>
    public static int TotalRegenStacks(Creature creature) =>
        (int)(creature.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.RegenPower>()
            + creature.GetPowerAmount<Powers.LifeEssencePower>());

    /// <summary>Generic Apply helper for any power type.</summary>
    public static Task ApplyPower<T>(PlayerChoiceContext ctx, Creature target, decimal amount, Creature applier, CardModel source)
        where T : PowerModel
    {
        return PowerCmd.Apply<T>(ctx, target, amount, applier, source);
    }
}