using System;
using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.OnEnded))]
public static class GumBlessResetPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        try
        {
            GumBlessRelic.ResetSelection();
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[GumBlessResetPatch] Failed to reset GumBlessRelic after run end: {ex.Message}");
        }
    }
}