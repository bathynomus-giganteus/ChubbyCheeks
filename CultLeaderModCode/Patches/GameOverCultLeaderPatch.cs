using System;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;
using CultLeaderMod.CultLeaderModCode.Character;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class GameOverCultLeaderPatch
{
    private const string CombatDeathText = "艾里亚斯被无尽的冰雪覆盖";

    [HarmonyPatch(typeof(NGameOverScreen), "InitializeBannerAndQuote")]
    [HarmonyPostfix]
    private static void InitializeBannerAndQuotePostfix(NGameOverScreen __instance)
    {
        try
        {
            if (!ShouldApplyCombatDeathOverride(__instance))
                return;

            var deathQuote = GetField<MegaRichTextLabel>(__instance, "_deathQuote");
            if (deathQuote != null)
                deathQuote.Text = CombatDeathText;

            SetField(__instance, "_encounterQuote", CombatDeathText);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[GameOverCultLeaderPatch] Quote override failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(NGameOverScreen), nameof(NGameOverScreen.AfterOverlayOpened))]
    [HarmonyPostfix]
    private static void AfterOverlayOpenedPostfix(NGameOverScreen __instance)
    {
        try
        {
            if (!ShouldRotateCultLeaderPortrait(__instance))
                return;

            var localPlayer = GetField<Player>(__instance, "_localPlayer");
            if (localPlayer == null)
                return;

            var visuals = FindLocalPlayerVisuals(__instance, localPlayer);
            if (visuals != null)
                visuals.Rotation = -Mathf.Pi * 0.5f;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[GameOverCultLeaderPatch] Portrait rotation failed: {ex.Message}");
        }
    }

    private static bool ShouldApplyCombatDeathOverride(NGameOverScreen screen)
    {
        var history = GetField<RunHistory>(screen, "_history");
        if (history == null || history.Win || history.KilledByEncounter == ModelId.none)
            return false;

        var localPlayer = GetField<Player>(screen, "_localPlayer");
        return localPlayer?.Character is CultLeaderModCharacter;
    }

    private static bool ShouldRotateCultLeaderPortrait(NGameOverScreen screen)
    {
        var history = GetField<RunHistory>(screen, "_history");
        if (history == null || history.Win)
            return false;

        var isAbandoned = history.WasAbandoned || RunManager.Instance?.IsAbandoned == true;
        var isCombatDeath = history.KilledByEncounter != ModelId.none;
        if (!isAbandoned && !isCombatDeath)
            return false;

        var localPlayer = GetField<Player>(screen, "_localPlayer");
        return localPlayer?.Character is CultLeaderModCharacter;
    }

    private static NCreatureVisuals? FindLocalPlayerVisuals(NGameOverScreen screen, Player localPlayer)
    {
        if (NCombatRoom.Instance != null)
        {
            var playerNode = NCombatRoom.Instance.CreatureNodes
                .FirstOrDefault(node => node.Entity == localPlayer.Creature);
            if (playerNode != null)
                return playerNode.Visuals;
        }

        var container = GetField<Control>(screen, "_creatureContainer");
        if (container == null)
            return null;

        var playerVisuals = container.GetChildren().OfType<NCreatureVisuals>().ToList();
        if (playerVisuals.Count == 0)
            return null;

        var runState = GetField<RunState>(screen, "_runState");
        var slotIndex = runState?.GetPlayerSlotIndex(localPlayer) ?? -1;
        if (slotIndex >= 0 && slotIndex < playerVisuals.Count)
            return playerVisuals[slotIndex];

        return playerVisuals[0];
    }

    private static T? GetField<T>(object instance, string fieldName) where T : class
    {
        var field = typeof(NGameOverScreen).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field?.GetValue(instance) as T;
    }

    private static void SetField(object instance, string fieldName, object value)
    {
        var field = typeof(NGameOverScreen).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(instance, value);
    }
}
