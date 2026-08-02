using CultLeaderMod.CultLeaderModCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Patches;

/// <summary>
/// When a creature has ElderForm, redirect all base buff applications to elder buffs.
/// Patches PowerCmd.Apply(PowerModel power, Creature target, ...)
/// </summary>
[HarmonyPatch]
public static class ElderFormRedirectPatch
{
    private static MethodBase TargetMethod() =>
        typeof(PowerCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Apply"
                && !m.IsGenericMethod
                && m.GetParameters().Length >= 4
                && m.GetParameters()[0].ParameterType == typeof(PlayerChoiceContext)
                && m.GetParameters()[1].ParameterType == typeof(PowerModel));

    /// <summary>
    /// Prefix: if target has ElderForm and power is a base buff, swap to elder buff.
    /// </summary>
    private static void Prefix(ref PowerModel power, Creature target)
    {
        if (target.GetPowerAmount<ElderFormPower>() <= 0) return;
        if (!ElderFormHelper.IsBaseBuff(power)) return;

        var elderType = ElderFormHelper.GetElderType(power.GetType());
        if (elderType == null) return;

        var elderPower = (PowerModel)Activator.CreateInstance(elderType)!;
        var amountProp = typeof(PowerModel).GetProperty("Amount",
            BindingFlags.Public | BindingFlags.Instance)!;
        amountProp.SetValue(elderPower, power.Amount);
        power = elderPower;
    }
}