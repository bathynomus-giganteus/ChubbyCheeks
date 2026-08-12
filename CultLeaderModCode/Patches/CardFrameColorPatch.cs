using CultLeaderMod.CultLeaderModCode.CardTags;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class CardFrameColorPatch
{
    private const string OldDebugBorderRootName = "CultLeaderPersonalityBorder";

    [HarmonyPatch(typeof(NCard), "Reload")]
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        try
        {
            RemoveOldDebugBorder(__instance);

            var model = __instance.Model;
            if (model == null)
                return;

            var tags = model.Tags;
            Material? mat = null;

            // Rainbow first: card with ALL 5 personality tags.
            if (
                tags.Contains(CultLeaderCardTags.Pure)
                && tags.Contains(CultLeaderCardTags.Calm)
                && tags.Contains(CultLeaderCardTags.Frenzy)
                && tags.Contains(CultLeaderCardTags.Lively)
                && tags.Contains(CultLeaderCardTags.Melancholy)
            )
                mat = CultLeaderFrameColors.Rainbow;
            else if (tags.Contains(CultLeaderCardTags.Pure))
                mat = CultLeaderFrameColors.Pure;
            else if (tags.Contains(CultLeaderCardTags.Calm))
                mat = CultLeaderFrameColors.Calm;
            else if (tags.Contains(CultLeaderCardTags.Frenzy))
                mat = CultLeaderFrameColors.Frenzy;
            else if (tags.Contains(CultLeaderCardTags.Lively))
                mat = CultLeaderFrameColors.Lively;
            else if (tags.Contains(CultLeaderCardTags.Melancholy))
                mat = CultLeaderFrameColors.Melancholy;
            else
                return;

            if (!GodotObject.IsInstanceValid(mat))
                return;

            var frame = __instance.GetNodeOrNull<TextureRect>("%Frame");
            if (frame != null)
                frame.Material = mat;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardFrameColorPatch] Failed to recolor card frame: {ex.Message}");
        }
    }

    private static void RemoveOldDebugBorder(NCard card)
    {
        var oldBorder = card.GetNodeOrNull<Control>(OldDebugBorderRootName);
        if (oldBorder != null)
            oldBorder.QueueFree();
    }
}
