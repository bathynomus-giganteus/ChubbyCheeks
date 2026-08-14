using CultLeaderMod.CultLeaderModCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class FullBlockCombatStartPatch
{
    [HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCombatStart))]
    [HarmonyPostfix]
    private static void Postfix(ICombatState? combatState, ref Task __result)
    {
        __result = ApplyTrackers(combatState, __result);
    }

    private static async Task ApplyTrackers(ICombatState? combatState, Task original)
    {
        await (original ?? Task.CompletedTask);

        if (combatState == null)
            return;

        foreach (var creature in combatState.PlayerCreatures)
        {
            await FullBlockCounterPower.EnsureTracker(
                new ThrowingPlayerChoiceContext(),
                creature,
                creature,
                null
            );
        }
    }
}
