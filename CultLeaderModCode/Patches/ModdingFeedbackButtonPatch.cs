using System;
using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;
using MegaCrit.Sts2.addons.mega_text;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(NModdingScreen), "_Ready")]
public static class ModdingFeedbackButtonPatch
{
    private const string FeedbackUrl = "https://github.com/bathynomus-giganteus/ChubbyCheeks/issues/new?template=mod_bug_report.yml";
    private static readonly HashSet<ulong> InjectedScreens = new();

    [HarmonyPostfix]
    private static void Postfix(NModdingScreen __instance)
    {
        try
        {
            if (!InjectedScreens.Add(__instance.GetInstanceId()))
                return;

            var getModsButton = __instance.GetNodeOrNull<NButton>("%GetModsButton");
            var makeModsButton = __instance.GetNodeOrNull<NButton>("%MakeModsButton");
            if (getModsButton == null || makeModsButton == null)
                return;

            var duplicate = makeModsButton.Duplicate() as NButton;
            if (duplicate == null)
                return;

            duplicate.Name = "CultLeaderModFeedbackButton";
            duplicate.Visible = true;
            duplicate.ZIndex = makeModsButton.ZIndex + 1;
            duplicate.Size = makeModsButton.Size;
            duplicate.Scale = makeModsButton.Scale;

            __instance.AddChild(duplicate);

            var size = makeModsButton.Size * makeModsButton.Scale;
            const float margin = 32f;
            float rightShift = size.Y * 0.5f;

            duplicate.AnchorLeft = 1f;
            duplicate.AnchorTop = 1f;
            duplicate.AnchorRight = 1f;
            duplicate.AnchorBottom = 1f;
            duplicate.GrowHorizontal = Control.GrowDirection.Begin;
            duplicate.GrowVertical = Control.GrowDirection.Begin;
            duplicate.OffsetLeft = -size.X - margin + rightShift;
            duplicate.OffsetTop = -size.Y - margin;
            duplicate.OffsetRight = -margin + rightShift;
            duplicate.OffsetBottom = -margin;

            var label = duplicate.GetNodeOrNull<MegaLabel>("Visuals/Label");
            label?.SetTextAutoSize("教主Mod反馈");

            duplicate.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnFeedbackPressed));

            Entry.Logger.Info($"[ModdingFeedbackButtonPatch] Added feedback button at {duplicate.GlobalPosition}.");
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[ModdingFeedbackButtonPatch] Failed to add feedback button: {ex.Message}");
        }
    }

    private static void OnFeedbackPressed(NButton button)
    {
        try
        {
            OS.ShellOpen(FeedbackUrl);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[ModdingFeedbackButtonPatch] Failed to open feedback URL: {ex.Message}");
        }
    }
}
