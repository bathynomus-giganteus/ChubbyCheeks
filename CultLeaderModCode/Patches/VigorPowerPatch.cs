using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CultLeaderMod.CultLeaderModCode.Patches;

/// <summary>
/// Fixes the base-game VigorPower bug where AfterAttack consumes stacks but
/// never clears the internal commandToModify binding. To make this robust even
/// when other powers (e.g. ForwardResolvePower) apply Vigor during attack
/// resolution, we clear the stale binding at the start of the next valid
/// BeforeAttack as well as after the original AfterAttack completes.
/// </summary>
[HarmonyPatch]
public static class VigorPowerPatch
{
    [HarmonyPatch(typeof(VigorPower), nameof(VigorPower.BeforeAttack))]
    [HarmonyPrefix]
    private static void BeforeAttackPrefix(VigorPower __instance, AttackCommand command)
    {
        if (command == null || command.Attacker != __instance.Owner || !command.DamageProps.IsPoweredAttack())
            return;

        ClearVigorCommand(__instance);
    }

    [HarmonyPatch(typeof(VigorPower), nameof(VigorPower.AfterAttack))]
    [HarmonyPostfix]
    private static void AfterAttackPostfix(VigorPower __instance, ref Task __result)
    {
        __result = RunAfterAttack(__result, __instance);
    }

    private static async Task RunAfterAttack(Task original, VigorPower vigor)
    {
        await original;
        ClearVigorCommand(vigor);
    }

    private static void ClearVigorCommand(VigorPower vigor)
    {
        var type = typeof(VigorPower);
        var dataType = type.GetNestedType("Data", System.Reflection.BindingFlags.NonPublic);
        if (dataType == null)
            return;

        var field = dataType.GetField("commandToModify", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (field == null)
            return;

        var getInternalData = type.GetMethod("GetInternalData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (getInternalData == null)
            return;

        try
        {
            var generic = getInternalData.MakeGenericMethod(dataType);
            var data = generic.Invoke(vigor, null);
            field.SetValue(data, null);
        }
        catch
        {
        }
    }
}